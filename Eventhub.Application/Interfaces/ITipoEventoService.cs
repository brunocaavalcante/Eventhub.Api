using Eventhub.Application.DTOs;

namespace Eventhub.Application.Interfaces;

public interface ITipoEventoService
{
    Task<IEnumerable<TipoEventoDto>> ListarTodosAsync();
}
