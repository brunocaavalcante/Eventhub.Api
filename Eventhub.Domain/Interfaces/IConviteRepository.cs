using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface IConviteRepository : IRepository<Convite>
{
    // Métodos adicionais se necessário
}

public interface IEnvioConviteRepository : IRepository<EnvioConvite>
{
    Task<IEnumerable<EnvioConvite>> GetConfirmadosByEventoAsync(int idEvento);
}