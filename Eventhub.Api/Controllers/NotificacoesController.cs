using Eventhub.Api.Models;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de notificações
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacoesController : BaseController
{
    private readonly INotificacaoService _notificacaoService;
    private readonly IUsuarioRepository _usuarioRepository;

    public NotificacoesController(INotificacaoService notificacaoService, IUsuarioRepository usuarioRepository)
    {
        _notificacaoService = notificacaoService;
        _usuarioRepository = usuarioRepository;
    }

    private async Task<int> ObterIdUsuarioLogadoAsync()
    {
        // Como o [Authorize] já validou o token, o claim "sub" sempre existirá
        var auth0Id = User.FindFirst("sub")?.Value 
                   ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        if (string.IsNullOrEmpty(auth0Id))
            throw new ExceptionValidation("Claim de identificação não encontrado no token.", true);

        // Busca o usuário no banco pelo Auth0 ID (KeycloakId)
        var usuario = await _usuarioRepository.GetByKeycloakIdAsync(auth0Id);
        
        if (usuario == null)
            throw new ExceptionValidation("Usuário não cadastrado no sistema.", true);
        
        return usuario.Id;
    }

    /// <summary>
    /// Obtém todas as notificações do usuário logado
    /// </summary>
    [HttpGet("usuario/{idUsuario}")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<NotificacaoResponseDto>>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> ObterPorUsuario(int idUsuario)
    {
        try
        {
            var userId = await ObterIdUsuarioLogadoAsync();
            if (userId != idUsuario)
            {
                throw new ExceptionValidation("Você não tem permissão para acessar essas notificações.", true);
            }

            var notificacoes = await _notificacaoService.ObterPorUsuarioAsync(idUsuario);
            return CustomResponse(notificacoes);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém notificações não lidas do usuário logado
    /// </summary>
    [HttpGet("usuario/{idUsuario}/nao-lidas")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<NotificacaoResponseDto>>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> ObterNaoLidas(int idUsuario)
    {
        try
        {
            // Validar que o usuário só pode acessar suas próprias notificações
            var userId = await ObterIdUsuarioLogadoAsync();
            if (userId != idUsuario)
            {
                throw new ExceptionValidation("Você não tem permissão para acessar essas notificações.", true);
            }

            var notificacoes = await _notificacaoService.ObterNaoLidasAsync(idUsuario);
            return CustomResponse(notificacoes);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém uma notificação específica por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomResponse<NotificacaoResponseDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        try
        {
            var userId = await ObterIdUsuarioLogadoAsync();
            var notificacao = await _notificacaoService.ObterPorIdAsync(id, userId);
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
    /// Cria uma nova notificação
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomResponse<NotificacaoResponseDto>), 201)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Criar([FromBody] NotificacaoCreateDto dto)
    {
        try
        {
            var notificacao = await _notificacaoService.AdicionarAsync(dto);
            return CustomResponse(notificacao, 201);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Marca uma notificação como lida
    /// </summary>
    [HttpPatch("{id}/marcar-lida")]
    [ProducesResponseType(typeof(CustomResponse<NotificacaoResponseDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    [ProducesResponseType(typeof(CustomResponse<object>), 401)]
    public async Task<IActionResult> MarcarComoLida(int id)
    {
        try
        {
            var userId = await ObterIdUsuarioLogadoAsync();
            var notificacao = await _notificacaoService.MarcarComoLidaAsync(id, userId);
            return CustomResponse(notificacao);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Marca todas as notificações de um usuário como lidas
    /// </summary>
    [HttpPatch("usuario/{idUsuario}/marcar-todas-lidas")]
    [ProducesResponseType(typeof(CustomResponse<object>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> MarcarTodasComoLidas(int idUsuario)
    {
        try
        {
            // Validar que o usuário só pode marcar suas próprias notificações
            var userId = await ObterIdUsuarioLogadoAsync();
            if (userId != idUsuario)
            {
                throw new ExceptionValidation("Você não tem permissão para executar esta ação.", true);
            }

            await _notificacaoService.MarcarTodasComoLidasAsync(idUsuario);
            return CustomResponse<object>(new { mensagem = "Todas as notificações foram marcadas como lidas." });
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
    [ProducesResponseType(typeof(CustomResponse<object>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            var userId = await ObterIdUsuarioLogadoAsync();
            await _notificacaoService.RemoverAsync(id, userId);
            return CustomResponse<object>(new { mensagem = "Notificação removida com sucesso." });
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }
}
