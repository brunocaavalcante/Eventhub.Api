using Eventhub.Api.Models;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParticipantesController : BaseController
{
    private readonly IParticipanteService _participanteService;
    private readonly IEnvioConviteService _envioConviteService;

    public ParticipantesController(IParticipanteService participanteService, IEnvioConviteService envioConviteService)
    {
        _participanteService = participanteService;
        _envioConviteService = envioConviteService;
    }
    [HttpGet("evento/{idEvento}/confirmados")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<EnvioConviteDto>>), 200)]
    public async Task<IActionResult> ObterConfirmados(int idEvento)
    {
        try
        {
            var confirmados = await _envioConviteService.GetConfirmadosByEventoAsync(idEvento);
            return CustomResponse(confirmados);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpGet("evento/{idEvento}")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<ParticipanteDto>>), 200)]
    public async Task<IActionResult> ObterPorEvento(int idEvento)
    {
        try
        {
            var participantes = await _participanteService.ObterPorEventoAsync(idEvento);
            return CustomResponse(participantes);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomResponse<ParticipanteDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        try
        {
            var participante = await _participanteService.ObterPorIdAsync(id);
            if (participante == null)
                return CustomResponse<object>(404, "Participante não encontrado.");

            return CustomResponse(participante);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomResponse<ParticipanteDto>), 201)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Criar([FromBody] CreateParticipanteDto participante)
    {
        try
        {
            var participanteCriado = await _participanteService.AdicionarAsync(participante);
            return CustomResponse(participanteCriado, 201);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomResponse<ParticipanteDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UpdateParticipanteDto participante)
    {
        try
        {
            if (id != participante.Id)
                return CustomResponse<object>(400, "ID do participante não corresponde.");

            var participanteAtualizado = await _participanteService.AtualizarAsync(participante);
            return CustomResponse(participanteAtualizado);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomResponse<object>), 204)]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            await _participanteService.RemoverAsync(id);
            return CustomResponse<object>(new { }, 204);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }
}
