using Eventhub.Application.DTOs;

namespace Eventhub.Application.Interfaces;

public interface IEnvioConviteService
{
    Task<EnvioConviteDto> EnviarAsync(EnvioConviteDto dto);
    Task<IEnumerable<EnvioConviteDto>> GetConfirmadosByEventoAsync(int idEvento);
}