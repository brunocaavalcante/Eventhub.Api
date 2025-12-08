using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class PerfilService : IPerfilService
{
    private readonly IPerfilRepository _perfilRepository;
    private readonly IMapper _mapper;

    public PerfilService(IPerfilRepository perfilRepository, IMapper mapper)
    {
        _perfilRepository = perfilRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PerfilDto>> ObterPerfisAtivosAsync()
    {
        var perfis = await _perfilRepository.GetPerfisAtivosAsync();
        return _mapper.Map<IEnumerable<PerfilDto>>(perfis);
    }
}