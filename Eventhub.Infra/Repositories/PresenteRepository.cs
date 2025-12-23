using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Infra.Repositories;

public class PresenteRepository : Repository<Presente>, IPresenteRepository
{
    public PresenteRepository(EventhubDbContext context) : base(context) { }

    public async Task<IEnumerable<CategoriaPresente>> GetByCategoryAsync()
    {
        return await _context.CategoriaPresentes.AsNoTracking().ToListAsync();
    }
}