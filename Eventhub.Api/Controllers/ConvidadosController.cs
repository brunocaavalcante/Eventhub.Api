using Eventhub.Api.Models;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConvidadosController : BaseController
{
    private readonly IConvidadosService _convidadosService;
    private readonly IConvidadosRepository _convidadosRepository;

    public ConvidadosController(IConvidadosService convidadosService, IConvidadosRepository convidadosRepository)
    {
        _convidadosService = convidadosService;
        _convidadosRepository = convidadosRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<Convidados>>), 200)]
    public async Task<IActionResult> ObterTodos()
    {
        try
        {
            var convidados = await _convidadosRepository.GetAllAsync();
            return CustomResponse(convidados);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomResponse<Convidados>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        try
        {
            var convidado = await _convidadosRepository.GetByIdAsync(id);
            if (convidado == null)
                return CustomResponse<object>(404, "Convidado não encontrado.");

            return CustomResponse(convidado);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpGet("evento/{idEvento}")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<Convidados>>), 200)]
    public async Task<IActionResult> ObterPorEvento(int idEvento)
    {
        try
        {
            var convidados = await _convidadosService.ObterPorEventoAsync(idEvento);
            return CustomResponse(convidados);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomResponse<Convidados>), 201)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Criar([FromBody] Convidados convidado)
    {
        try
        {
            var convidadoCreated = await _convidadosService.AdicionarAsync(convidado);
            return CustomResponse(convidadoCreated, 201);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomResponse<Convidados>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] Convidados convidado)
    {
        try
        {
            if (id != convidado.Id)
                return CustomResponse<object>(400, "ID do convidado não corresponde.");

            var convidadoAtualizado = await _convidadosService.AtualizarAsync(convidado);
            return CustomResponse(convidadoAtualizado);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomResponse<object>), 204)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            await _convidadosService.RemoverAsync(id);
            return CustomResponse<object>(new { }, 204);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }
}
