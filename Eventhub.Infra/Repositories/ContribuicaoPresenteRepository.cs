using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
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
            .Where(c => c.IdPresente == idPresente && c.IdStatusContribuicao == (int)StatusContribuicaoEnum.Confirmado)
            .SumAsync(c => (decimal?)c.Valor);

        return total ?? 0m;
    }
    
    public async Task<IEnumerable<StatusContribuicao>> GetAllStatusAsync()
    {
        return await _context.Set<StatusContribuicao>().ToListAsync();
    }
}
