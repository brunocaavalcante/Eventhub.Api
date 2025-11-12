using Eventhub.Domain.Entities;

namespace Eventhub.Application.Interfaces;

public interface IGaleriaService
{
    Task<Galeria> AdicionarAsync(Galeria galeria);
    Task<Galeria> AtualizarAsync(Galeria galeria);
    Task RemoverAsync(int id);
    Task<IEnumerable<Galeria>> ObterPorEventoAsync(int idEvento);
}
