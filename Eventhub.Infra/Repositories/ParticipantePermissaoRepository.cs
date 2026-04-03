using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Infra.Repositories;

public class ParticipantePermissaoRepository : Repository<ParticipantePermissao>, IParticipantePermissaoRepository
{
    public ParticipantePermissaoRepository(EventhubDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ParticipantePermissao>> GetByParticipanteAsync(int idParticipante)
    {
        return await _dbSet
            .Where(p => p.IdParticipante == idParticipante)
            .Include(p => p.Participante)
            .Include(p => p.Permissao)
            .ToListAsync();
    }

    public async Task<ParticipantePermissao?> GetByParticipantePermissaoAsync(int idParticipante, int idPermissao)
    {
        return await _dbSet
            .Include(p => p.Participante)
            .Include(p => p.Permissao)
            .FirstOrDefaultAsync(p => p.IdParticipante == idParticipante 
                                   && p.IdPermissao == idPermissao);
    }

    public async Task<bool> ExistsAsync(int idParticipante, int idPermissao)
    {
        return await _dbSet
            .AnyAsync(p => p.IdParticipante == idParticipante 
                        && p.IdPermissao == idPermissao);
    }
}
