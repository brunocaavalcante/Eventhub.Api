using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;

namespace Eventhub.Domain.Interfaces;

public interface IGaleriaRepository : IRepository<Galeria>
{
    Task<IEnumerable<Galeria>> GetByEventoAsync(int idEvento);
    Task<bool> ExistsByTipoAsync(int idEvento, GaleriaTipo tipo, int? ignoreId = null);
}
