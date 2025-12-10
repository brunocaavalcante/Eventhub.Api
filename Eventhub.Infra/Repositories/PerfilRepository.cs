using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Infra.Repositories;

public class PerfilRepository : Repository<Perfil>, IPerfilRepository
{
    public PerfilRepository(EventhubDbContext context) : base(context) { }

    public async Task<IEnumerable<Perfil>> GetPerfisAtivosAsync()
    {
        return await _dbSet.Where(p => p.Status == 'A').ToListAsync();
    }

    public async Task<Perfil> ObterPermissoesPerfil(int idPerfil)
    {
        return await _dbSet
            .Include(p => p.PerfilPermissoes)
                .ThenInclude(pp => pp.Permissao)
                .ThenInclude(m => m.Modulo)
            .FirstOrDefaultAsync(p => p.Id == idPerfil) ?? new Perfil();
    }
}
