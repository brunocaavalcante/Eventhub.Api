using Eventhub.Api.Models;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventosController : BaseController
{
    private readonly IEventoService _eventoService;
    private readonly IEventoRepository _eventoRepository;

    public EventosController(IEventoService eventoService, IEventoRepository eventoRepository)
    {
        _eventoService = eventoService;
        _eventoRepository = eventoRepository;
    }

    /// <summary>
    /// Obtém todos os eventos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<Evento>>), 200)]
    public async Task<IActionResult> ObterTodos()
    {
        try
        {
            var eventos = await _eventoRepository.GetAllAsync();
            return CustomResponse(eventos);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém um evento por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomResponse<Evento>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        try
        {
            var evento = await _eventoRepository.GetByIdAsync(id);
            if (evento == null)
                return CustomResponse<object>(404, "Evento não encontrado.");

            return CustomResponse(evento);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém eventos por usuário criador
    /// </summary>
    [HttpGet("usuario/{idUsuario}")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<Evento>>), 200)]
    public async Task<IActionResult> ObterPorUsuario(int idUsuario)
    {
        try
        {
            var eventos = await _eventoRepository.GetByUsuarioAsync(idUsuario);
            return CustomResponse(eventos);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém eventos por status
    /// </summary>
    [HttpGet("status/{idStatus}")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<Evento>>), 200)]
    public async Task<IActionResult> ObterPorStatus(int idStatus)
    {
        try
        {
            var eventos = await _eventoRepository.GetByStatusAsync(idStatus);
            return CustomResponse(eventos);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém eventos por tipo
    /// </summary>
    [HttpGet("tipo/{idTipo}")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<Evento>>), 200)]
    public async Task<IActionResult> ObterPorTipo(int idTipo)
    {
        try
        {
            var eventos = await _eventoRepository.GetByTipoAsync(idTipo);
            return CustomResponse(eventos);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém eventos ativos de um usuário
    /// </summary>
    [HttpGet("usuario/{idUsuario}/ativos")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<Evento>>), 200)]
    public async Task<IActionResult> ObterEventosAtivos(int idUsuario)
    {
        try
        {
            var eventos = await _eventoRepository.GetEventosAtivosByUsuarioAsync(idUsuario);
            return CustomResponse(eventos);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Cria um novo evento
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomResponse<Evento>), 201)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Criar([FromBody] Evento evento)
    {
        try
        {
            var eventoCreated = await _eventoService.AdicionarAsync(evento);
            return CustomResponse(eventoCreated, 201);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Atualiza um evento existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomResponse<Evento>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] Evento evento)
    {
        try
        {
            if (id != evento.Id)
                return CustomResponse<object>(400, "ID do evento não corresponde.");

            var eventoAtualizado = await _eventoService.AtualizarAsync(evento);
            return CustomResponse(eventoAtualizado);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Remove um evento
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomResponse<object>), 204)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            await _eventoService.RemoverAsync(id);
            return CustomResponse<object>(new { }, 204);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }
}
