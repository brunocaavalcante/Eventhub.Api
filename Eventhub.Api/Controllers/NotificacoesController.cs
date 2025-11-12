using Eventhub.Api.Models;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacoesController : BaseController
{
    private readonly INotificacaoService _notificacaoService;
    private readonly INotificacaoRepository _notificacaoRepository;

    public NotificacoesController(INotificacaoService notificacaoService, INotificacaoRepository notificacaoRepository)
    {
        _notificacaoService = notificacaoService;
        _notificacaoRepository = notificacaoRepository;
    }

    /// <summary>
    /// Obtém todas as notificações
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<Notificacao>>), 200)]
    public async Task<IActionResult> ObterTodas()
    {
        try
        {
            var notificacoes = await _notificacaoRepository.GetAllAsync();
            return CustomResponse(notificacoes);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém uma notificação por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomResponse<Notificacao>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        try
        {
            var notificacao = await _notificacaoRepository.GetByIdAsync(id);
            if (notificacao == null)
                return CustomResponse<object>(404, "Notificação não encontrada.");

            return CustomResponse(notificacao);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém notificações de um usuário
    /// </summary>
    [HttpGet("usuario/{idUsuario}")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<Notificacao>>), 200)]
    public async Task<IActionResult> ObterPorUsuario(int idUsuario)
    {
        try
        {
            var notificacoes = await _notificacaoService.ObterPorUsuarioAsync(idUsuario);
            return CustomResponse(notificacoes);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém notificações não lidas de um usuário
    /// </summary>
    [HttpGet("usuario/{idUsuario}/nao-lidas")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<Notificacao>>), 200)]
    public async Task<IActionResult> ObterNaoLidas(int idUsuario)
    {
        try
        {
            var notificacoes = await _notificacaoService.ObterNaoLidasAsync(idUsuario);
            return CustomResponse(notificacoes);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Cria uma nova notificação
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomResponse<Notificacao>), 201)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Criar([FromBody] Notificacao notificacao)
    {
        try
        {
            var notificacaoCreated = await _notificacaoService.AdicionarAsync(notificacao);
            return CustomResponse(notificacaoCreated, 201);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Atualiza uma notificação existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomResponse<Notificacao>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] Notificacao notificacao)
    {
        try
        {
            if (id != notificacao.Id)
                return CustomResponse<object>(400, "ID da notificação não corresponde.");

            var notificacaoAtualizada = await _notificacaoService.AtualizarAsync(notificacao);
            return CustomResponse(notificacaoAtualizada);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Remove uma notificação
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomResponse<object>), 204)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            await _notificacaoService.RemoverAsync(id);
            return CustomResponse<object>(new { }, 204);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }
}
