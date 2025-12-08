using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Eventhub.Application.Services;

public class AuthService : BaseService, IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordSecurity _passwordSecurity;
    private readonly HttpClient _httpClient;
    private readonly string _keycloakTokenUrl;
    private readonly string _keycloakLogoutUrl;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public AuthService(
        IConfiguration configuration,
        IUsuarioRepository usuarioRepository,
        IPasswordSecurity passwordSecurity,
        IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _usuarioRepository = usuarioRepository;
        _passwordSecurity = passwordSecurity;
        _httpClient = httpClientFactory.CreateClient();

        var authority = _configuration["Keycloak:Authority"] ?? throw new InvalidOperationException("Keycloak Authority não configurado");
        _keycloakTokenUrl = $"{authority}/protocol/openid-connect/token";
        _keycloakLogoutUrl = $"{authority}/protocol/openid-connect/logout";

        _clientId = _configuration["Keycloak:ClientId"] ?? throw new InvalidOperationException("Keycloak ClientId não configurado");
        _clientSecret = _configuration["Keycloak:ClientSecret"] ?? string.Empty;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
    {
        if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
            throw new ExceptionValidation("Email e senha são obrigatórios.");

        var usuario = await _usuarioRepository.GetByEmailAsync(loginRequest.Email);
        if (usuario == null)
            throw new ExceptionValidation("Usuário não encontrado.");

        var tokenResponse = await ObterTokenKeycloakAsync(loginRequest.Email, loginRequest.Password);

        return new LoginResponseDto
        {
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            ExpiresIn = tokenResponse.ExpiresIn,
            TokenType = tokenResponse.TokenType,
            Usuario = MapearUsuario(usuario)
        };
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ExceptionValidation("Refresh token é obrigatório.");

        try
        {
            var parameters = new Dictionary<string, string>
            {
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "refresh_token", refreshToken }
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync(_keycloakLogoutUrl, content);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            throw new ExceptionValidation($"Erro ao fazer logout: {ex.Message}");
        }
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenRequest.RefreshToken))
            throw new ExceptionValidation("Refresh token é obrigatório.");

        try
        {
            var parameters = new Dictionary<string, string>
            {
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshTokenRequest.RefreshToken }
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync(_keycloakTokenUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ExceptionValidation($"Erro ao renovar token: {errorContent}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponseDto>(jsonResponse);

            if (tokenResponse == null)
                throw new ExceptionValidation("Resposta inválida do servidor de autenticação.");

            return new LoginResponseDto
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresIn = tokenResponse.ExpiresIn,
                TokenType = tokenResponse.TokenType
            };
        }
        catch (Exception ex) when (ex is not ExceptionValidation)
        {
            throw new ExceptionValidation($"Erro ao renovar token: {ex.Message}");
        }
    }

    private async Task<KeycloakTokenResponseDto> ObterTokenKeycloakAsync(string email, string password)
    {
        try
        {
            string senhaDescriptografada;
            try
            {
                senhaDescriptografada = _passwordSecurity.DecryptPassword(password);
            }
            catch (Exception ex)
            {
                throw new ExceptionValidation("Erro ao processar a senha.", ex);
            }

            var parameters = new Dictionary<string, string>
            {
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "grant_type", "password" },
                { "username", email },
                { "password", senhaDescriptografada }
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync(_keycloakTokenUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new ExceptionValidation("Credenciais inválidas.");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponseDto>(jsonResponse);

            if (tokenResponse == null)
                throw new ExceptionValidation("Resposta inválida do servidor de autenticação.");

            return tokenResponse;
        }
        catch (Exception ex) when (ex is not ExceptionValidation)
        {
            throw new ExceptionValidation($"Erro ao autenticar: {ex.Message}");
        }
    }

    private static string? ObterKeycloakIdDoToken(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            return null;

        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(accessToken))
            return null;

        var jwt = handler.ReadJwtToken(accessToken);
        return jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
    }

    private static UsuarioInfoDto MapearUsuario(Usuario usuario)
    {
        return new UsuarioInfoDto
        {
            Id = usuario.Id,
            KeycloakId = usuario.KeycloakId,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Telefone = usuario.Telefone,
            FotoBase64 = usuario.Foto != null ? usuario.Foto.Base64 : "",
            DataCadastro = usuario.DataCadastro,
            Status = usuario.Status
        };
    }
}
