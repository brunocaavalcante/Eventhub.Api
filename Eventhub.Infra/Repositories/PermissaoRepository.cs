using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Infra.Repositories;

public class PermissaoRepository : Repository<Permissao>, IPermissaoRepository
{
    public PermissaoRepository(EventhubDbContext context) : base(context)
    {
    }

    public async Task<Permissao?> GetByChaveAsync(string chave)
    {
        return await _dbSet
            .Include(p => p.Modulo)
            .FirstOrDefaultAsync(p => p.Chave == chave);
    }

    public async Task<IEnumerable<Permissao>> GetByModuloAsync(int idModulo)
    {
        return await _dbSet
            .Where(p => p.IdModulo == idModulo)
            .Include(p => p.Modulo)
            .ToListAsync();
    }
}
