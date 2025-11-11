using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Infra.Repositories;

public class GaleriaRepository : Repository<Galeria>, IGaleriaRepository
{
    public GaleriaRepository(EventhubDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Galeria>> GetByEventoAsync(int idEvento)
    {
        return await _dbSet
            .Where(g => g.IdEvento == idEvento)
            .Include(g => g.Foto)
            .OrderBy(g => g.Ordem)
            .ToListAsync();
    }
}
