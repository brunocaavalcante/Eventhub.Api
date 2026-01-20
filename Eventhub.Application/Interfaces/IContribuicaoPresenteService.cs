using Eventhub.Application.DTOs;

namespace Eventhub.Application.Interfaces;

public interface IContribuicaoPresenteService
{
    Task<ContribuicaoPresenteDto> CriarAsync(CreateContribuicaoPresenteDto dto);
}
