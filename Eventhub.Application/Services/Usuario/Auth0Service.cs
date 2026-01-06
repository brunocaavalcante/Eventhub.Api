using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Eventhub.Application.Services;

public class Auth0Service : IAuth0Service
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly string _domain;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _managementClientId;
    private readonly string _managementClientSecret;
    private readonly string _managementApiUrl;
    private readonly string _managementApiAudience;
    private readonly string _tokenUrl;

    public Auth0Service(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();

        _domain = _configuration["Auth0:Domain"] ?? throw new InvalidOperationException("Auth0 Domain não configurado");
        _clientId = _configuration["Auth0:ClientId"] ?? throw new InvalidOperationException("Auth0 ClientId não configurado");
        _clientSecret = _configuration["Auth0:ClientSecret"] ?? throw new InvalidOperationException("Auth0 ClientSecret não configurado");
        
        // Usa credenciais separadas para Management API se configuradas, senão usa as principais
        _managementClientId = _configuration["Auth0:ManagementClientId"] ?? _clientId;
        _managementClientSecret = _configuration["Auth0:ManagementClientSecret"] ?? _clientSecret;
        
        _managementApiUrl = _configuration["Auth0:ManagementApiUrl"] ?? $"https://{_domain}/api/v2";
        _managementApiAudience = _configuration["Auth0:ManagementApiAudience"] ?? $"https://{_domain}/api/v2/";
        _tokenUrl = _configuration["Auth0:TokenUrl"] ?? $"https://{_domain}/oauth/token";
    }

    public async Task<string> ObterManagementTokenAsync()
    {
        try
        {
            var payload = new
            {
                client_id = _managementClientId,
                client_secret = _managementClientSecret,
                audience = _managementApiAudience,
                grant_type = "client_credentials"
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_tokenUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ExceptionValidation($"Erro ao obter token de gerenciamento Auth0: {error}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<Auth0TokenResponseDto>(jsonResponse);

            return tokenResponse?.AccessToken ?? throw new ExceptionValidation("Token de gerenciamento não obtido.");
        }
        catch (Exception ex) when (ex is not ExceptionValidation)
        {
            throw new ExceptionValidation($"Erro ao autenticar no Auth0 Management API: {ex.Message}");
        }
    }

    public async Task<string> CriarUsuarioAsync(string nome, string email, string password)
    {
        try
        {
            var token = await ObterManagementTokenAsync();

            var auth0User = new Auth0UserDto
            {
                Email = email,
                Name = nome,
                Password = password,
                EmailVerified = true,
                Connection = "Username-Password-Authentication",
                VerifyEmail = false
            };

            var jsonContent = JsonSerializer.Serialize(auth0User);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.PostAsync($"{_managementApiUrl}/users", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ExceptionValidation($"Erro ao criar usuário no Auth0: {error}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var createdUser = JsonSerializer.Deserialize<Auth0UserDto>(jsonResponse);

            return createdUser?.UserId ?? string.Empty;
        }
        catch (Exception ex) when (ex is not ExceptionValidation)
        {
            throw new ExceptionValidation($"Erro ao criar usuário no Auth0: {ex.Message}");
        }
    }

    public async Task AtualizarUsuarioAsync(string email, string novoNome, string? novoEmail = null)
    {
        try
        {
            var token = await ObterManagementTokenAsync();
            var userId = await ObterUsuarioIdPorEmailAsync(email, token);

            if (string.IsNullOrEmpty(userId))
                throw new ExceptionValidation("Usuário não encontrado no Auth0.");

            var updateUser = new
            {
                email = novoEmail ?? email,
                name = novoNome,
                email_verified = true
            };

            var jsonContent = JsonSerializer.Serialize(updateUser);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.PatchAsync($"{_managementApiUrl}/users/{Uri.EscapeDataString(userId)}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ExceptionValidation($"Erro ao atualizar usuário no Auth0: {error}");
            }
        }
        catch (Exception ex) when (ex is not ExceptionValidation)
        {
            throw new ExceptionValidation($"Erro ao atualizar usuário no Auth0: {ex.Message}");
        }
    }

    public async Task DeletarUsuarioAsync(string email)
    {
        try
        {
            var token = await ObterManagementTokenAsync();
            var userId = await ObterUsuarioIdPorEmailAsync(email, token);

            if (string.IsNullOrEmpty(userId))
                throw new ExceptionValidation("Usuário não encontrado no Auth0.");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.DeleteAsync($"{_managementApiUrl}/users/{Uri.EscapeDataString(userId)}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ExceptionValidation($"Erro ao deletar usuário no Auth0: {error}");
            }
        }
        catch (Exception ex) when (ex is not ExceptionValidation)
        {
            throw new ExceptionValidation($"Erro ao deletar usuário no Auth0: {ex.Message}");
        }
    }

    private async Task<string?> ObterUsuarioIdPorEmailAsync(string email, string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            // Auth0 usa Lucene query syntax
            var query = Uri.EscapeDataString($"email:\"{email}\"");
            var response = await _httpClient.GetAsync($"{_managementApiUrl}/users?q={query}&search_engine=v3");

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<List<Auth0UserDto>>(jsonResponse);

            return users?.FirstOrDefault()?.UserId;
        }
        catch
        {
            return null;
        }
    }
}
