using Eventhub.Application.DTOs;

namespace Eventhub.Application.Interfaces;

public interface IPerfilService
{
    Task<IEnumerable<PerfilDto>> ObterPerfisAtivosAsync();
}
