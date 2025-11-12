using Eventhub.Domain.Entities;

namespace Eventhub.Application.Interfaces;

public interface IEventoService
{
    Task<Evento> AdicionarAsync(Evento evento);
    Task<Evento> AtualizarAsync(Evento evento);
    Task RemoverAsync(int id);
}
