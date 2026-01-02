using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface IEventoRepository : IRepository<Evento>
{
    Task<Evento?> GetByIdAsync(int id);
    Task<IEnumerable<Evento>> GetByStatusAsync(int idStatus);
    Task<IEnumerable<Evento>> GetByTipoAsync(int idTipo);
    Task<IEnumerable<Evento>> GetEventosByUsuarioAsync(int idUsuario);
}
