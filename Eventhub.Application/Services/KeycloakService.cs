using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Eventhub.Application.Services;

public class KeycloakService : IKeycloakService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly string _keycloakAdminUrl;
    private readonly string _keycloakTokenUrl;
    private readonly string _adminUsername;
    private readonly string _adminPassword;
    private readonly string _adminClientId;
    private readonly string _adminRealm;
    private readonly string _targetRealm;

    public KeycloakService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();

        var authority = _configuration["Keycloak:Authority"] ?? throw new InvalidOperationException("Keycloak Authority não configurado");
        _targetRealm = _configuration["Keycloak:Realm"] ?? throw new InvalidOperationException("Keycloak Realm não configurado");
        
        // Configuração do admin (usa master realm para obter token)
        var adminSection = _configuration.GetSection("Keycloak:Admin");
        _adminRealm = adminSection["Realm"] ?? "master";
        _adminClientId = adminSection["ClientId"] ?? "admin-cli";
        _adminUsername = adminSection["Username"] ?? "admin";
        _adminPassword = adminSection["Password"] ?? throw new InvalidOperationException("Keycloak AdminPassword não configurado");

        var baseUrl = authority.Contains("/realms/")
            ? authority.Substring(0, authority.IndexOf("/realms/"))
            : authority;
        
        _keycloakAdminUrl = $"{baseUrl}/admin/realms/{_targetRealm}/users";
        _keycloakTokenUrl = $"{baseUrl}/realms/{_adminRealm}/protocol/openid-connect/token";
    }

    public async Task<string> ObterAccessTokenAdminAsync()
    {
        try
        {
            var parameters = new Dictionary<string, string>
            {
                { "client_id", _adminClientId },
                { "username", _adminUsername },
                { "password", _adminPassword },
                { "grant_type", "password" }
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync(_keycloakTokenUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ExceptionValidation($"Erro ao obter token admin: {error}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponseDto>(jsonResponse);

            return tokenResponse?.AccessToken ?? throw new ExceptionValidation("Token admin não obtido.");
        }
        catch (Exception ex) when (ex is not ExceptionValidation)
        {
            throw new ExceptionValidation($"Erro ao autenticar admin: {ex.Message}");
        }
    }

    public async Task<string> CriarUsuarioAsync(string nome, string email, string password)
    {
        try
        {
            var token = await ObterAccessTokenAdminAsync();

            var nomePartes = nome.Split(' ', 2);
            var firstName = nomePartes[0];
            var lastName = nomePartes.Length > 1 ? nomePartes[1] : string.Empty;

            var keycloakUser = new KeycloakUserDto
            {
                Username = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Enabled = true,
                Credentials = new List<KeycloakCredentialDto>
                {
                    new KeycloakCredentialDto
                    {
                        Type = "password",
                        Value = password,
                        Temporary = false
                    }
                }
            };

            var jsonContent = JsonSerializer.Serialize(keycloakUser);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsync(_keycloakAdminUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ExceptionValidation($"Erro ao criar usuário no Keycloak: {error}");
            }

            var locationHeader = response.Headers.Location?.ToString();
            var userId = locationHeader?.Split('/').LastOrDefault() ?? string.Empty;

            return userId;
        }
        catch (Exception ex) when (ex is not ExceptionValidation)
        {
            throw new ExceptionValidation($"Erro ao criar usuário no Keycloak: {ex.Message}");
        }
    }

    public async Task AtualizarUsuarioAsync(string email, string novoNome, string? novoEmail = null)
    {
        try
        {
            var token = await ObterAccessTokenAdminAsync();
            var userId = await ObterUsuarioIdPorEmailAsync(email, token);

            if (string.IsNullOrEmpty(userId))
                throw new ExceptionValidation("Usuário não encontrado no Keycloak.");

            var nomePartes = novoNome.Split(' ', 2);
            var firstName = nomePartes[0];
            var lastName = nomePartes.Length > 1 ? nomePartes[1] : string.Empty;

            var updateUser = new KeycloakUserDto
            {
                Username = novoEmail ?? email,
                Email = novoEmail ?? email,
                FirstName = firstName,
                LastName = lastName,
                Enabled = true
            };

            var jsonContent = JsonSerializer.Serialize(updateUser);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PutAsync($"{_keycloakAdminUrl}/{userId}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ExceptionValidation($"Erro ao atualizar usuário no Keycloak: {error}");
            }
        }
        catch (Exception ex) when (ex is not ExceptionValidation)
        {
            throw new ExceptionValidation($"Erro ao atualizar usuário no Keycloak: {ex.Message}");
        }
    }

    public async Task DeletarUsuarioAsync(string email)
    {
        try
        {
            var token = await ObterAccessTokenAdminAsync();
            var userId = await ObterUsuarioIdPorEmailAsync(email, token);

            if (string.IsNullOrEmpty(userId))
                throw new ExceptionValidation("Usuário não encontrado no Keycloak.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"{_keycloakAdminUrl}/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ExceptionValidation($"Erro ao deletar usuário no Keycloak: {error}");
            }
        }
        catch (Exception ex) when (ex is not ExceptionValidation)
        {
            throw new ExceptionValidation($"Erro ao deletar usuário no Keycloak: {ex.Message}");
        }
    }

    private async Task<string?> ObterUsuarioIdPorEmailAsync(string email, string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_keycloakAdminUrl}?email={email}");

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(jsonResponse);

            return users?.FirstOrDefault()?["id"].GetString();
        }
        catch
        {
            return null;
        }
    }
}
