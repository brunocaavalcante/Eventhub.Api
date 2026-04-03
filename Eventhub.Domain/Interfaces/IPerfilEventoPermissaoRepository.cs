using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface IPerfilEventoPermissaoRepository : IRepository<PerfilEventoPermissao>
{
    Task<IEnumerable<PerfilEventoPermissao>> GetByPerfilEventoAsync(int idPerfil, int idEvento);
    Task<IEnumerable<PerfilEventoPermissao>> GetByEventoAsync(int idEvento);
    Task<PerfilEventoPermissao?> GetByPerfilEventoPermissaoAsync(int idPerfil, int idEvento, int idPermissao);
    Task<bool> ExistsAsync(int idPerfil, int idEvento, int idPermissao);
}
