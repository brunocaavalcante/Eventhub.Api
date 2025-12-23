using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface IPresenteRepository : IRepository<Presente>
{
    Task<IEnumerable<CategoriaPresente>> GetByCategoryAsync();
}
