using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Infra.Repositories;

public class ContribuicaoPresenteRepository : Repository<ContribuicaoPresente>, IContribuicaoPresenteRepository
{
    public ContribuicaoPresenteRepository(EventhubDbContext context) : base(context) { }

    public async Task<decimal> GetTotalContribuidoAsync(int idPresente)
    {
        var total = await _dbSet
            .Where(c => c.IdPresente == idPresente)
            .SumAsync(c => (decimal?)c.Valor);

        return total ?? 0m;
    }
}
