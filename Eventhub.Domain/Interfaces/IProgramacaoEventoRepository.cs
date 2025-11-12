using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface IProgramacaoEventoRepository : IRepository<ProgramacaoEvento>
{
    Task<IEnumerable<ProgramacaoEvento>> GetByEventoAsync(int idEvento);
}
