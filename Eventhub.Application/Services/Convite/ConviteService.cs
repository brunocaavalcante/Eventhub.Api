using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Validations;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class ConviteService : BaseService, IConviteService
{
    private readonly IConviteRepository _conviteRepository;
    private readonly IFotosService _fotosService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ConviteService(IConviteRepository conviteRepository,
    IFotosService fotosService, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _conviteRepository = conviteRepository;
        _fotosService = fotosService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ConviteDto> CriarAsync(CreateConviteDto dto)
    {
        ExecutarValidacao(new CreateConviteValidation(), dto);

        var foto = await _fotosService.UploadAsync(dto.Foto);

        var convite = _mapper.Map<Convite>(dto);
        convite.IdFoto = foto.Id;

        await _conviteRepository.AddAsync(convite);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<ConviteDto>(convite);
    }
}
