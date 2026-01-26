using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface IContribuicaoPresenteRepository : IRepository<ContribuicaoPresente>
{
    Task<decimal> GetTotalContribuidoAsync(int idPresente);
}
