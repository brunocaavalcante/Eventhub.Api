using Eventhub.Api.Models;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConviteController : BaseController
{
    private readonly IConviteService _conviteService;
    private readonly IEnvioConviteService _envioConviteService;
    private readonly IUnitOfWork _unitOfWork;

    public ConviteController(IConviteService conviteService, IEnvioConviteService envioConviteService, IUnitOfWork unitOfWork)
    {
        _conviteService = conviteService;
        _envioConviteService = envioConviteService;
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomResponse<CreateConviteDto>), 201)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Criar([FromBody] CreateConviteDto dto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var convite = await _conviteService.CriarAsync(dto);

            await _unitOfWork.CommitTransactionAsync();

            return CustomResponse(convite, 201);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return TratarErros(ex);
        }
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