using Eventhub.Application.DTOs;
using Eventhub.Domain.Entities;

namespace Eventhub.Application.Interfaces;

public interface INotificacaoService
{
    Task<NotificacaoResponseDto> AdicionarAsync(NotificacaoCreateDto dto);
    Task RemoverAsync(int id, int idUsuario);
    Task<NotificacaoResponseDto?> ObterPorIdAsync(int id, int idUsuario);
    Task<IEnumerable<NotificacaoResponseDto>> ObterPorUsuarioAsync(int idUsuario);
    Task<IEnumerable<NotificacaoResponseDto>> ObterNaoLidasAsync(int idUsuario);
    Task<NotificacaoResponseDto> MarcarComoLidaAsync(int id, int idUsuario);
    Task MarcarTodasComoLidasAsync(int idUsuario);
}
