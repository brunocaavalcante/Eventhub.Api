using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface IProgramacaoEventoRepository : IRepository<ProgramacaoEvento>
{
    Task<ProgramacaoEvento?> GetByIdWithIncludesAsync(int id);
    Task<IEnumerable<ProgramacaoEvento>> GetByEventoAsync(int idEvento);
}
