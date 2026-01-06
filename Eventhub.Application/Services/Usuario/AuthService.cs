using System.IdentityModel.Tokens.Jwt;
using System.Text;
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
    private readonly string _tokenUrl;
    private readonly string _audience;
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

        var domain = _configuration["Auth0:Domain"] ?? throw new InvalidOperationException("Auth0 Domain não configurado");
        _tokenUrl = _configuration["Auth0:TokenUrl"] ?? $"https://{domain}/oauth/token";
        _audience = _configuration["Auth0:Audience"] ?? throw new InvalidOperationException("Auth0 Audience não configurado");
        _clientId = _configuration["Auth0:ClientId"] ?? throw new InvalidOperationException("Auth0 ClientId não configurado");
        _clientSecret = _configuration["Auth0:ClientSecret"] ?? throw new InvalidOperationException("Auth0 ClientSecret não configurado");
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
    {
        if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
            throw new ExceptionValidation("Email e senha são obrigatórios.");

        var usuario = await _usuarioRepository.GetByEmailAsync(loginRequest.Email);
        if (usuario == null)
            throw new ExceptionValidation("Usuário não encontrado.");

        var tokenResponse = await ObterTokenAuth0Async(loginRequest.Email, loginRequest.Password);

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
        // Auth0 não requer logout explícito do lado servidor
        // O token expira naturalmente ou pode ser revogado via Management API se necessário
        return await Task.FromResult(true);
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenRequest.RefreshToken))
            throw new ExceptionValidation("Refresh token é obrigatório.");

        try
        {
            var payload = new
            {
                client_id = _clientId,
                client_secret = _clientSecret,
                grant_type = "refresh_token",
                refresh_token = refreshTokenRequest.RefreshToken
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_tokenUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ExceptionValidation($"Erro ao renovar token: {errorContent}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<Auth0TokenResponseDto>(jsonResponse);

            if (tokenResponse == null)
                throw new ExceptionValidation("Resposta inválida do servidor de autenticação.");

            return new LoginResponseDto
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken ?? refreshTokenRequest.RefreshToken,
                ExpiresIn = tokenResponse.ExpiresIn,
                TokenType = tokenResponse.TokenType
            };
        }
        catch (Exception ex) when (ex is not ExceptionValidation)
        {
            throw new ExceptionValidation($"Erro ao renovar token: {ex.Message}");
        }
    }

    private async Task<Auth0TokenResponseDto> ObterTokenAuth0Async(string email, string password)
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

            var payload = new
            {
                client_id = _clientId,
                client_secret = _clientSecret,
                grant_type = "password",
                username = email,
                password = senhaDescriptografada,
                scope = "openid profile email offline_access",
                realm = "Username-Password-Authentication"
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_tokenUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ExceptionValidation($"Credenciais inválidas. {error}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<Auth0TokenResponseDto>(jsonResponse);

            if (tokenResponse == null)
                throw new ExceptionValidation("Resposta inválida do Auth0.");

            return tokenResponse;
        }
        catch (Exception ex) when (ex is not ExceptionValidation)
        {
            throw new ExceptionValidation($"Erro ao autenticar com Auth0: {ex.Message}");
        }
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
