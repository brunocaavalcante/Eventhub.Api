using Eventhub.Application.DTOs;

namespace Eventhub.Application.Interfaces;

public interface IFotosService
{
    Task<FotoDto> UploadAsync(UploadFotoDto dto);
    Task<FotoDto> UpdateAsync(UpdateFotoDto dto);
    Task<FotoDto?> GetByIdAsync(int id);
    Task RemoverAsync(int id);
}
