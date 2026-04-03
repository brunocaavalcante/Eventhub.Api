using Eventhub.Application.DTOs;

namespace Eventhub.Application.Interfaces;

public interface IPresenteService
{
    Task<PresenteDto> AdicionarAsync(CreatePresenteDto dto);
    Task<PresenteDto> AtualizarAsync(UpdatePresenteDto dto);
    Task RemoverAsync(int id);
    Task<PresenteDto?> ObterPorIdAsync(int id);
    Task<PresenteDetalhesDto?> ObterDetalhesPorIdAsync(int id);
    Task<IEnumerable<PresenteDto>> ListarTodosAsync(int idEvento);
    Task<IEnumerable<CategoriaPresenteDto>> ListarCategoriaPresentesAsync();
    Task ReservarPresenteAsync(int idPresente, ReservarPresenteDto dto);
    Task CancelarReservaPresenteAsync(int idPresente, CancelarReservaPresenteDto dto);
    Task<IEnumerable<StatusPresenteDto>> ObterStatusPresentesAsync();
}