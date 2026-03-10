using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface IParticipantePermissaoRepository : IRepository<ParticipantePermissao>
{
    Task<IEnumerable<ParticipantePermissao>> GetByParticipanteAsync(int idParticipante);
    Task<ParticipantePermissao?> GetByParticipantePermissaoAsync(int idParticipante, int idPermissao);
    Task<bool> ExistsAsync(int idParticipante, int idPermissao);
}
