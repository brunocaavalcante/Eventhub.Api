using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Infra.Repositories;

public class ProgramacaoEventoRepository : Repository<ProgramacaoEvento>, IProgramacaoEventoRepository
{
    public ProgramacaoEventoRepository(EventhubDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ProgramacaoEvento>> GetByEventoAsync(int idEvento)
    {
        return await _dbSet
            .Where(p => p.IdEvento == idEvento)
            .Include(p => p.Foto)
            .Include(p => p.Responsaveis)
                .ThenInclude(r => r.Usuario)
            .OrderBy(p => p.Data)
            .ToListAsync();
    }
}
