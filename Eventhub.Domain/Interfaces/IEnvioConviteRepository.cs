using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface IEnvioConviteRepository : IRepository<EnvioConvite>
{
    Task<IEnumerable<EnvioConvite>> GetConfirmadosByEventoAsync(int idEvento);
}