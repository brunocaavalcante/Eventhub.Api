using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Infra.Repositories;

public class EnvioConviteRepository : Repository<EnvioConvite>, IEnvioConviteRepository
{
    public EnvioConviteRepository(EventhubDbContext context) : base(context) {}

    public async Task<IEnumerable<EnvioConvite>> GetConfirmadosByEventoAsync(int idEvento)
    {
        return await _dbSet
            .Where(e => e.IdEvento == idEvento && e.Status == "Confirmado")
            .Include(e => e.Participante)
            .ToListAsync();
    }
}