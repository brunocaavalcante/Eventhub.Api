using System.Net;
using Eventhub.Api.Models;
using Eventhub.Application.DTOs;
using Eventhub.Application.DTOs.Evento;
using Eventhub.Application.Helpers;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
using Eventhub.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventosController : BaseController
{
    private readonly IEventoService _eventoService;
    private readonly IEventoRepository _eventoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPerfilEventoPermissaoService _perfilEventoPermissaoService;

    public EventosController(
        IEventoService eventoService,
        IEventoRepository eventoRepository,
        IUnitOfWork unitOfWork,
        IPerfilEventoPermissaoService perfilEventoPermissaoService)
    {
        _eventoService = eventoService;
        _eventoRepository = eventoRepository;
        _unitOfWork = unitOfWork;
        _perfilEventoPermissaoService = perfilEventoPermissaoService;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomResponse<EventoDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        try
        {
            var evento = await _eventoService.ObterPorIdAsync(id);
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
    /// Obtém eventos do usuário (criados e participações)
    /// </summary>
    [HttpGet("usuario/{idUsuario}")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<EventoAtivoDto>>), 200)]
    public async Task<IActionResult> ObterPorUsuario(int idUsuario)
    {
        try
        {
            var eventos = await _eventoService.ObterEventosPorUsuarioAsync(idUsuario);
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
    /// Obtém eventos por tipo
    /// </summary>
    [HttpGet("status")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<StatusEventoDto>>), 200)]
    public async Task<IActionResult> ObterStatusEventos()
    {
        try
        {
            var eventos = await _eventoService.ObterStatusEventosAsync();
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
    [ProducesResponseType(typeof(CustomResponse<EventoCadastroDto>), 201)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Criar([FromBody] EventoCadastroDto evento)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var eventoCreated = await _eventoService.AdicionarAsync(evento);
            await _unitOfWork.CommitTransactionAsync();

            return CustomResponse(eventoCreated, 201);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Atualiza um evento existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomResponse<EventoDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UpdateEventoDto evento)
    {
        try
        {
            if (id != evento.Id)
                return CustomResponse<object>(400, "ID do evento não corresponde.");

            await _unitOfWork.BeginTransactionAsync();
            var eventoAtualizado = await _eventoService.AtualizarAsync(evento);
            await _unitOfWork.CommitTransactionAsync();
            
            return CustomResponse(eventoAtualizado);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Remove um evento
    /// </summary>
    /// <remarks>
    /// Remove permanentemente um evento e todos os dados relacionados, com as seguintes validações:
    /// - Não pode ter iniciado (DataInicio já passou)
    /// - Não pode estar concluído
    /// - Não pode ter contribuições confirmadas ou em análise
    /// - Cancela automaticamente contribuições pendentes
    /// - Notifica todos os participantes sobre a exclusão
    /// - Remove em cascade: Galerias, Fotos, Programações, Notificações, Participantes, Presentes, Contribuições, PixEventos, Permissões
    /// </remarks>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomResponse<object>), 204)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            await _eventoService.RemoverAsync(id);
            await _unitOfWork.CommitTransactionAsync();
            
            return CustomResponse<object>(new { Mensagem = "Evento excluído com sucesso. Notificações enviadas aos participantes." }, 204);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return TratarErros(ex);
        }
    }

    [HttpGet("token/{token:guid}")]
    [ProducesResponseType(typeof(CustomResponse<EventoAtivoDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> ObterPorToken(Guid token)
    {
        try
        {
            var evento = await _eventoService.ObterPorTokenAsync(token);
            if (evento == null)
                return CustomResponse<object>(404, "Evento não encontrado ou não está mais ativo.");

            return CustomResponse(evento);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém as permissões padrão de visibilidade do evento (perfil Convidado)
    /// </summary>
    [HttpGet("{idEvento}/permissoes-padrao")]
    [ProducesResponseType(typeof(CustomResponse<ConfiguracaoVisibilidadeDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> ObterPermissoesPadrao(int idEvento)
    {
        try
        {
            // Sempre trabalha com perfil Convidado
            var idPerfilConvidado = (int)EnumPerfil.Convidado;
            var permissoes = await _perfilEventoPermissaoService.ObterConfiguracaoPerfilAsync(idEvento, idPerfilConvidado);
            var configuracao = PermissaoMapper.PermissoesToDto(permissoes);
            return CustomResponse(configuracao);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Atualiza as permissões padrão de visibilidade do evento (perfil Convidado)
    /// </summary>
    [HttpPut("{idEvento}/permissoes-padrao")]
    [ProducesResponseType(typeof(CustomResponse<object>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> AtualizarPermissoesPadrao(int idEvento, [FromBody] ConfiguracaoVisibilidadeDto configuracao)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            // Sempre trabalha com perfil Convidado
            var idPerfilConvidado = (int)EnumPerfil.Convidado;
            var permissoes = PermissaoMapper.DtoToPermissoes(configuracao);
            await _perfilEventoPermissaoService.ConfigurarPermissoesPerfilAsync(idEvento, idPerfilConvidado, permissoes);
            await _unitOfWork.CommitTransactionAsync();

            return CustomResponse(new { Mensagem = "Permissões atualizadas com sucesso." });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Cancela um evento
    /// </summary>
    /// <remarks>
    /// Cancela o evento e executa as seguintes ações:
    /// - Bloqueia se houver contribuições confirmadas ou em análise
    /// - Cancela automaticamente contribuições pendentes
    /// - Libera reservas de presentes
    /// - Altera status do evento para Cancelado
    /// - Envia notificações para todos os participantes com a justificativa
    /// </remarks>
    [HttpPatch("{id}/cancelar")]
    [ProducesResponseType(typeof(CustomResponse<object>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> CancelarEvento(int id, [FromBody] CancelarEventoDto dto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            await _eventoService.CancelarEventoAsync(id, dto);
            await _unitOfWork.CommitTransactionAsync();

            return CustomResponse(new { Mensagem = "Evento cancelado com sucesso. Notificações enviadas aos participantes." });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Reativa um evento cancelado
    /// </summary>
    /// <remarks>
    /// Reativa um evento que foi cancelado, com as seguintes validações:
    /// - Apenas eventos cancelados podem ser reativados
    /// - Data do evento não pode ter passado
    /// - Não pode ter contribuições estornadas
    /// - Status volta para Agendado ou Ativo conforme a data
    /// </remarks>
    [HttpPatch("{id}/reativar")]
    [ProducesResponseType(typeof(CustomResponse<object>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> ReativarEvento(int id)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            await _eventoService.ReativarEventoAsync(id);
            await _unitOfWork.CommitTransactionAsync();

            return CustomResponse(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return TratarErros(ex);
        }
    }
}
