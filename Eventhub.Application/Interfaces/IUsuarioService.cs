using Eventhub.Application.DTOs;
using Eventhub.Domain.Entities;

namespace Eventhub.Application.Interfaces;

public interface IUsuarioService
{
    Task<Usuario> AdicionarAsync(CreateUsuarioDto createUsuarioDto);
    Task<Usuario> AtualizarAsync(Usuario usuario);
    Task RemoverAsync(int id);
    Task<Usuario?> ObterPorEmailAsync(string email);
}
