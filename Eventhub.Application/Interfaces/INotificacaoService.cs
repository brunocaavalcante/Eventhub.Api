using Eventhub.Domain.Entities;

namespace Eventhub.Application.Interfaces;

public interface INotificacaoService
{
    Task<Notificacao> AdicionarAsync(Notificacao notificacao);
    Task<Notificacao> AtualizarAsync(Notificacao notificacao);
    Task RemoverAsync(int id);
    Task<IEnumerable<Notificacao>> ObterPorUsuarioAsync(int idUsuario);
    Task<IEnumerable<Notificacao>> ObterNaoLidasAsync(int idUsuario);
}
