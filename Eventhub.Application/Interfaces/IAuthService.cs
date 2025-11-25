using Eventhub.Application.DTOs;

namespace Eventhub.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
    Task<bool> LogoutAsync(string refreshToken);
    Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest);
}
