using Eventhub.Api.Models;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomResponse<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
    {
        try
        {
            var response = await _authService.LoginAsync(loginRequest);
            return CustomResponse(response);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(CustomResponse<object>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var success = await _authService.LogoutAsync(request.RefreshToken);
            if (success)
                return CustomResponse("Logout realizado com sucesso.");

            return CustomResponse<object>(400, "Erro ao realizar logout.");
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomResponse<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request);
            return CustomResponse(response);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }
}
