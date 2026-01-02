using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<Usuario?> GetByKeycloakIdAsync(string keycloakId);
}
