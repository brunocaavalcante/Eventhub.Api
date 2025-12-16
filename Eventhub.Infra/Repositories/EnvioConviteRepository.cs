using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
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
            .Include(e => e.Participante)
            .Where(e => e.IdEvento == idEvento && e.IdStatusEnvioConvite == (int)EnumStatusEnvioConvite.Confirmado)
            .ToListAsync();
    }
}