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

    public async Task<ContribuicaoPresenteDto> AtualizarAsync(UpdateContribuicaoPresenteDto dto)
    {
        ExecutarValidacao(new UpdateContribuicaoPresenteValidation(), dto);

        var entity = await _contribuicaoPresenteRepository.GetByIdAsync(dto.Id);
        if (entity == null)
            throw new ExceptionValidation("Contribuição não encontrada.");

        _mapper.Map(dto, entity);

        _contribuicaoPresenteRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ContribuicaoPresenteDto>(entity);
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

    public async Task CancelarAsync(CancelarContribuicaoPresenteDto dto)
    {
        ExecutarValidacao(new CancelarContribuicaoPresenteValidation(), dto);

        var contribuicao = await _contribuicaoPresenteRepository.GetByIdAsync(dto.IdContribuicao);
        if (contribuicao == null)
            throw new ExceptionValidation("Contribuição não encontrada.");

        if (contribuicao.IdStatusContribuicao == (int)StatusContribuicaoEnum.Cancelado)
            throw new ExceptionValidation("A contribuição já está cancelada.");

        contribuicao.IdStatusContribuicao = (int)StatusContribuicaoEnum.Cancelado;
        contribuicao.Justificativa = dto.Justificativa;

        _contribuicaoPresenteRepository.Update(contribuicao);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<StatusContribuicaoDto>> ObterStatusAsync()
    {
        var statusEntities = await _contribuicaoPresenteRepository.GetAllStatusAsync();
        return _mapper.Map<IEnumerable<StatusContribuicaoDto>>(statusEntities);
    }
}
