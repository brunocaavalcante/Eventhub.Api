using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class ConviteService : IConviteService
{
    private readonly IConviteRepository _conviteRepository;
    private readonly IMapper _mapper;

    public ConviteService(IConviteRepository conviteRepository, IMapper mapper)
    {
        _conviteRepository = conviteRepository;
        _mapper = mapper;
    }

    public async Task<ConviteDto> CriarAsync(ConviteDto dto)
    {
        var convite = _mapper.Map<Convite>(dto);
        await _conviteRepository.AddAsync(convite);
        return _mapper.Map<ConviteDto>(convite);
    }
}
