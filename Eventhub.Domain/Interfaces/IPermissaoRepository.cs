using Eventhub.Domain.Entities;

namespace Eventhub.Domain.Interfaces;

public interface IPermissaoRepository : IRepository<Permissao>
{
    Task<Permissao?> GetByChaveAsync(string chave);
    Task<IEnumerable<Permissao>> GetByModuloAsync(int idModulo);
}
