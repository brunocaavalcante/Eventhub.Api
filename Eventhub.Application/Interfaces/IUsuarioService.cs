using Eventhub.Domain.Entities;

namespace Eventhub.Application.Interfaces;

public interface IUsuarioService
{
    Task<Usuario> AdicionarAsync(Usuario usuario);
    Task<Usuario> AtualizarAsync(Usuario usuario);
    Task RemoverAsync(int id);
    Task<Usuario?> ObterPorEmailAsync(string email);
}
