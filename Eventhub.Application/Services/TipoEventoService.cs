using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Interfaces;
using AutoMapper;

namespace Eventhub.Application.Services;

public class TipoEventoService : ITipoEventoService
{
    private readonly ITipoEventoRepository _tipoEventoRepository;
    private readonly IMapper _mapper;

    public TipoEventoService(ITipoEventoRepository tipoEventoRepository, IMapper mapper)
    {
        _tipoEventoRepository = tipoEventoRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TipoEventoDto>> ListarTodosAsync()
    {
        var tipos = await _tipoEventoRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<TipoEventoDto>>(tipos);
    }
}
