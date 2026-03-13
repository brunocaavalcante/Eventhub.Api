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
    private readonly IParticipanteRepository _participanteRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEventoRepository _eventoRepository;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ContribuicaoPresenteService(
        IContribuicaoPresenteRepository contribuicaoPresenteRepository,
        IPresenteRepository presenteRepository,
        IFotosService fotosService,
        IParticipanteRepository participanteRepository,
        IUsuarioRepository usuarioRepository,
        IEventoRepository eventoRepository,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _contribuicaoPresenteRepository = contribuicaoPresenteRepository;
        _presenteRepository = presenteRepository;
        _fotosService = fotosService;
        _participanteRepository = participanteRepository;
        _usuarioRepository = usuarioRepository;
        _eventoRepository = eventoRepository;
        _emailService = emailService;
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

        var totalContribuido = await _contribuicaoPresenteRepository.GetTotalContribuidoAsync(contribuicao.IdPresente);
        var presente = await _presenteRepository.GetByIdAsync(contribuicao.IdPresente);
        if (presente == null) throw new ExceptionValidation("Presente não encontrado.");

        var novoStatus = StatusPresenteEnum.Disponivel;

        if (totalContribuido > 0)
            novoStatus = totalContribuido >= presente.Valor ? StatusPresenteEnum.Reservado : StatusPresenteEnum.EmArrecadacao;


        if (presente.IdStatus != (int)novoStatus)
        {
            presente.IdStatus = (int)novoStatus;
            _presenteRepository.Update(presente);
            await _unitOfWork.SaveChangesAsync();
        }

        // Enviar email para o contribuidor
        var participante = await _participanteRepository.GetByIdAsync(contribuicao.IdParticipante);
        if (participante != null)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(participante.IdUsuario);
            var evento = await _eventoRepository.GetByIdAsync(presente.IdEvento);
            
            if (usuario != null && evento != null && !string.IsNullOrWhiteSpace(usuario.Email))
            {
                await _emailService.EnviarEmailContribuicaoCanceladaAsync(
                    usuario.Email,
                    usuario.Nome,
                    presente.Nome,
                    evento.Nome,
                    contribuicao.Valor,
                    dto.Justificativa);
            }
        }
    }

    public async Task ConfirmarAsync(ConfirmarContribuicaoPresenteDto dto)
    {
        if (dto.IdContribuicao <= 0 || dto.IdPresente <= 0)
            throw new ExceptionValidation("Id da contribuição ou do presente inválido.");

        var contribuicao = await _contribuicaoPresenteRepository.GetByIdAsync(dto.IdContribuicao);
        if (contribuicao == null)
            throw new ExceptionValidation("Contribuição não encontrada.");

        if (contribuicao.IdStatusContribuicao == (int)StatusContribuicaoEnum.Confirmado)
            throw new ExceptionValidation("A contribuição já está confirmada.");

        contribuicao.IdStatusContribuicao = (int)StatusContribuicaoEnum.Confirmado;

        var presente = await _presenteRepository.GetByIdAsync(dto.IdPresente);
        if (presente == null)
            throw new ExceptionValidation("Presente não encontrado.");

        _contribuicaoPresenteRepository.Update(contribuicao);
        await _unitOfWork.SaveChangesAsync();

        var totalContribuido = await _contribuicaoPresenteRepository.GetTotalContribuidoAsync(dto.IdPresente);
        var novoStatus = totalContribuido >= presente.Valor ? StatusPresenteEnum.Reservado : StatusPresenteEnum.EmArrecadacao;

        if (presente.IdStatus != (int)novoStatus)
        {
            presente.IdStatus = (int)novoStatus;
            _presenteRepository.Update(presente);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<StatusContribuicaoDto>> ObterStatusAsync()
    {
        var statusEntities = await _contribuicaoPresenteRepository.GetAllStatusAsync();
        return _mapper.Map<IEnumerable<StatusContribuicaoDto>>(statusEntities);
    }
}
