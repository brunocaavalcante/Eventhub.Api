using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface INotificacaoRepository : IRepository<Notificacao>
{
    Task<IEnumerable<Notificacao>> GetByUsuarioDestinoAsync(int idUsuario);
    Task<IEnumerable<Notificacao>> GetNaoLidasByUsuarioAsync(int idUsuario);
}
