using Eventhub.Application.DTOs;

namespace Eventhub.Application.Interfaces;

public interface IConviteService
{
    Task<ConviteDto> CriarAsync(CreateConviteDto dto);
    Task<ConviteDto> ObterPorEventoAsync(int idEvento);
}
