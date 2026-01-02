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

    public async Task<Presente?> GetByIdCompletoAsync(int id)
    {
        return await _dbSet
                    .Include(p => p.Status)
                    .Include(p => p.Categoria)
                    .Include(p => p.Contribuicoes)
                    .Include(p => p.Galerias).ThenInclude(g => g.Foto)
                    .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Presente>> GetByEventIdAsync(int idEvento)
    {
        return await _context.Presentes
            .AsNoTracking()
            .Include(p => p.Categoria)
            .Include(p => p.Status)
            .Include(p => p.Galerias)
                .ThenInclude(g => g.Foto)
            .Where(p => p.IdEvento == idEvento)
            .ToListAsync();
    }
}