using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Infra.Repositories;

public class PixEventoRepository : Repository<PixEvento>, IPixEventoRepository
{
    public PixEventoRepository(EventhubDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PixEvento>> GetByEventoIdAsync(int idEvento)
    {
        return await _context.Set<PixEvento>()
            .Where(p => p.IdEvento == idEvento)
            .OrderByDescending(p => p.DataCadastro)
            .ToListAsync();
    }

    public async Task<PixEvento?> GetByEventoAndFinalidadeAsync(int idEvento, FinalidadePix finalidade)
    {
        return await _context.Set<PixEvento>()
            .FirstOrDefaultAsync(p => p.IdEvento == idEvento && p.Finalidade == finalidade);
    }
}
