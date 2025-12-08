using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class EnvioConviteService : IEnvioConviteService
{
    private readonly IEnvioConviteRepository _envioConviteRepository;
    private readonly IMapper _mapper;

    public EnvioConviteService(IEnvioConviteRepository envioConviteRepository, IMapper mapper)
    {
        _envioConviteRepository = envioConviteRepository;
        _mapper = mapper;
    }

    public async Task<EnvioConviteDto> EnviarAsync(EnvioConviteDto dto)
    {
        var envio = _mapper.Map<EnvioConvite>(dto);
        await _envioConviteRepository.AddAsync(envio);
        return _mapper.Map<EnvioConviteDto>(envio);
    }

    public async Task<IEnumerable<EnvioConviteDto>> GetConfirmadosByEventoAsync(int idEvento)
    {
        var envios = await _envioConviteRepository.GetConfirmadosByEventoAsync(idEvento);
        return _mapper.Map<IEnumerable<EnvioConviteDto>>(envios);
    }
}