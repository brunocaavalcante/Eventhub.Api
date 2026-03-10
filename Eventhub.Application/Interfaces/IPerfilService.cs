using Eventhub.Application.DTOs;

namespace Eventhub.Application.Interfaces;

public interface IPerfilService
{
    Task<IEnumerable<PerfilDto>> ObterPerfisAtivosAsync();
    Task<IEnumerable<ModuloDto>> ObterModulosPerfilAsync(int idPerfil, int idEvento);
}
