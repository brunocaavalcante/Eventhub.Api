using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;

namespace Eventhub.Domain.Interfaces;

public interface IPixEventoRepository : IRepository<PixEvento>
{
    Task<IEnumerable<PixEvento>> GetByEventoIdAsync(int idEvento);
    Task<PixEvento?> GetByEventoAndFinalidadeAsync(int idEvento, FinalidadePix finalidade);
}
