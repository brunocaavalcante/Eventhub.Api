using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Validations;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class ContribuicaoPresenteService : BaseService, IContribuicaoPresenteService
{
    private readonly IContribuicaoPresenteRepository _contribuicaoPresenteRepository;
    private readonly IPresenteRepository _presenteRepository;
    private readonly IFotosService _fotosService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ContribuicaoPresenteService(
        IContribuicaoPresenteRepository contribuicaoPresenteRepository,
        IPresenteRepository presenteRepository,
        IFotosService fotosService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _contribuicaoPresenteRepository = contribuicaoPresenteRepository;
        _presenteRepository = presenteRepository;
        _fotosService = fotosService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ContribuicaoPresenteDto> CriarAsync(CreateContribuicaoPresenteDto dto)
    {
        ExecutarValidacao(new CreateContribuicaoPresenteValidation(), dto);

        var presente = await _presenteRepository.GetByIdAsync(dto.IdPresente);
        if (presente == null)
            throw new ExceptionValidation("Presente não encontrado.");

        var foto = await _fotosService.UploadAsync(dto.Comprovante);

        var contribuicao = _mapper.Map<ContribuicaoPresente>(dto);
        contribuicao.IdStatusContribuicao = (int)StatusContribuicaoEnum.EmAnalise;
        contribuicao.IdFoto = foto.Id;
        contribuicao.DataCadastro = DateTime.UtcNow;

        await _contribuicaoPresenteRepository.AddAsync(contribuicao);
        await _unitOfWork.SaveChangesAsync();

        var totalContribuido = await _contribuicaoPresenteRepository.GetTotalContribuidoAsync(dto.IdPresente);
        var novoStatus = totalContribuido >= presente.Valor ? StatusPresenteEnum.Reservado : StatusPresenteEnum.EmArrecadacao;

        if (presente.IdStatus != (int)novoStatus)
        {
            presente.IdStatus = (int)novoStatus;
            _presenteRepository.Update(presente);
            await _unitOfWork.SaveChangesAsync();
        }

        return _mapper.Map<ContribuicaoPresenteDto>(contribuicao);
    }
}
