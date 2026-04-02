using Eventhub.Application.DTOs;

namespace Eventhub.Application.Interfaces;

public interface IProgramacaoEventoService
{
    Task<ProgramacaoEventoResponseDto> AdicionarAsync(ProgramacaoEventoCreateDto dto);
    Task<ProgramacaoEventoResponseDto> AtualizarAsync(ProgramacaoEventoUpdateDto dto);
    Task RemoverAsync(int id);
    Task<ProgramacaoEventoResponseDto> ObterPorIdAsync(int id);
    Task<IEnumerable<ProgramacaoEventoResponseDto>> ObterPorEventoAsync(int idEvento);
}
