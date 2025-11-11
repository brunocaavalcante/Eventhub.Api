using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface IConvidadosRepository : IRepository<Convidados>
{
    Task<IEnumerable<Convidados>> GetByEventoAsync(int idEvento);
    Task<Convidados?> GetByEmailAsync(string email);
}
