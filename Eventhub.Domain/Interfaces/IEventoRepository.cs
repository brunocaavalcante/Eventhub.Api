using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface IEventoRepository : IRepository<Evento>
{
    Task<IEnumerable<Evento>> GetByUsuarioAsync(int idUsuario);
    Task<IEnumerable<Evento>> GetByStatusAsync(int idStatus);
    Task<IEnumerable<Evento>> GetByTipoAsync(int idTipo);
    Task<IEnumerable<Evento>> GetEventosByUsuarioAsync(int idUsuario);
}
