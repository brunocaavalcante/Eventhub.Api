using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface IGaleriaRepository : IRepository<Galeria>
{
    Task<IEnumerable<Galeria>> GetByEventoAsync(int idEvento);
}
