using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface IPresenteRepository : IRepository<Presente>
{
    Task<IEnumerable<CategoriaPresente>> GetByCategoryAsync();
    Task<IEnumerable<Presente>> GetByEventIdAsync(int idEvento);
    Task<Presente?> GetByIdCompletoAsync(int id);
}
