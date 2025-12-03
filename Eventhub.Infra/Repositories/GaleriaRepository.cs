using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
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

    public async Task<bool> ExistsByTipoAsync(int idEvento, GaleriaTipo tipo, int? ignoreId = null)
    {
        var query = _dbSet.AsQueryable()
            .Where(g => g.IdEvento == idEvento && g.Tipo == tipo);

        if (ignoreId.HasValue)
        {
            query = query.Where(g => g.Id != ignoreId.Value);
        }

        return await query.AnyAsync();
    }
}
