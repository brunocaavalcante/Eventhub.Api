using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConviteController : ControllerBase
{
    private readonly IConviteService _conviteService;
    private readonly IEnvioConviteService _envioConviteService;

    public ConviteController(IConviteService conviteService, IEnvioConviteService envioConviteService)
    {
        _conviteService = conviteService;
        _envioConviteService = envioConviteService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ConviteDto dto)
    {
        var convite = await _conviteService.CriarAsync(dto);
        return CreatedAtAction(nameof(Criar), new { id = convite.Id }, convite);
    }

    [HttpPost("{id}/enviar")]
    public async Task<IActionResult> Enviar(int id, [FromBody] EnvioConviteDto dto)
    {
        dto.IdConvite = id;
        var envio = await _envioConviteService.EnviarAsync(dto);
        return Ok(envio);
    }

    [HttpGet("{id}/envios")]
    public async Task<IActionResult> GetEnvios(int id)
    {
        // Implementar busca de envios por convite
        return Ok();
    }
}