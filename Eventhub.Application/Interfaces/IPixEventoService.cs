using Eventhub.Application.DTOs;
using Eventhub.Domain.Enums;

namespace Eventhub.Application.Interfaces;

public interface IPixEventoService
{
    Task<PixEventoDto> CreateAsync(CreatePixEventoDto dto);
    Task<PixEventoDto> UpdateAsync(UpdatePixEventoDto dto);
    Task DeleteAsync(int id);
    Task<PixEventoDto?> GetByIdAsync(int id);
    Task<IEnumerable<PixEventoDto>> GetByEventoIdAsync(int idEvento);
    Task<PixEventoDto?> GetByEventoAndFinalidadeAsync(int idEvento, FinalidadePix finalidade);
}
