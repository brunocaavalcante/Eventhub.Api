using Eventhub.Domain.Entities;

namespace Eventhub.Application.Interfaces;

public interface IConvidadosService
{
    Task<Convidados> AdicionarAsync(Convidados convidado);
    Task<Convidados> AtualizarAsync(Convidados convidado);
    Task RemoverAsync(int id);
    Task<IEnumerable<Convidados>> ObterPorEventoAsync(int idEvento);
}
