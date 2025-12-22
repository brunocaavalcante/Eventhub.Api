using Eventhub.Application.DTOs;

namespace Eventhub.Application.Interfaces;

public interface IPresenteService
{
    Task<PresenteDto> AdicionarAsync(CreatePresenteDto dto);
    Task<PresenteDto> AtualizarAsync(UpdatePresenteDto dto);
    Task RemoverAsync(int id);
    Task<PresenteDto?> ObterPorIdAsync(int id);
    Task<IEnumerable<PresenteDto>> ListarTodosAsync();
}