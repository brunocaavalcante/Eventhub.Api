using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Infra.Repositories;

public class ConvidadosRepository : Repository<Convidados>, IConvidadosRepository
{
    public ConvidadosRepository(EventhubDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Convidados>> GetByEventoAsync(int idEvento)
    {
        return await _dbSet
            .Where(c => c.IdEvento == idEvento)
            .Include(c => c.Foto)
            .ToListAsync();
    }

    public async Task<Convidados?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Email == email);
    }
}
