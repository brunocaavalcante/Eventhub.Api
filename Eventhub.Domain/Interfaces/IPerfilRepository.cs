using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface IPerfilRepository : IRepository<Perfil>
{
    Task<IEnumerable<Perfil>> GetPerfisAtivosAsync();
}
