using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Infra.Repositories;

public class PerfilEventoPermissaoRepository : Repository<PerfilEventoPermissao>, IPerfilEventoPermissaoRepository
{
    public PerfilEventoPermissaoRepository(EventhubDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PerfilEventoPermissao>> GetByPerfilEventoAsync(int idPerfil, int idEvento)
    {
        return await _dbSet
            .Where(p => p.IdPerfil == idPerfil && p.IdEvento == idEvento)
            .Include(p => p.Perfil)
            .Include(p => p.Evento)
            .Include(p => p.Permissao)
            .ToListAsync();
    }

    public async Task<IEnumerable<PerfilEventoPermissao>> GetByEventoAsync(int idEvento)
    {
        return await _dbSet
            .Where(p => p.IdEvento == idEvento)
            .Include(p => p.Perfil)
            .Include(p => p.Permissao)
            .ToListAsync();
    }

    public async Task<PerfilEventoPermissao?> GetByPerfilEventoPermissaoAsync(int idPerfil, int idEvento, int idPermissao)
    {
        return await _dbSet
            .Include(p => p.Perfil)
            .Include(p => p.Evento)
            .Include(p => p.Permissao)
            .FirstOrDefaultAsync(p => p.IdPerfil == idPerfil 
                                   && p.IdEvento == idEvento 
                                   && p.IdPermissao == idPermissao);
    }

    public async Task<bool> ExistsAsync(int idPerfil, int idEvento, int idPermissao)
    {
        return await _dbSet
            .AnyAsync(p => p.IdPerfil == idPerfil 
                        && p.IdEvento == idEvento 
                        && p.IdPermissao == idPermissao);
    }
}
