using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface IParticipanteRepository : IRepository<Participante>
{
    Task<IEnumerable<Participante>> GetByEventoAsync(int idEvento);
    Task<Participante?> GetByIdWithDetailsAsync(int id);
    Task<Participante?> GetByUsuarioEventoWithDetailsAsync(int idParticipante, int idEvento);
    Task<bool> ExistsAsync(int idEvento, int idUsuario, int idPerfil, int? ignoreId = null);
}
