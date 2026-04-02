using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Validations;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class ProgramacaoEventoService : BaseService, IProgramacaoEventoService
{
    private readonly IProgramacaoEventoRepository _programacaoEventoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProgramacaoEventoService(
        IProgramacaoEventoRepository programacaoEventoRepository, 
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _programacaoEventoRepository = programacaoEventoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProgramacaoEventoResponseDto> AdicionarAsync(ProgramacaoEventoCreateDto dto)
    {
        var programacao = _mapper.Map<ProgramacaoEvento>(dto);
        programacao.DataCadastro = DateTime.UtcNow;
        
        if (programacao.Duracao == TimeSpan.Zero)
            programacao.Duracao = TimeSpan.FromHours(1);

        ExecutarValidacao(new ProgramacaoEventoValidation(), programacao);

        await _programacaoEventoRepository.AddAsync(programacao);
        await _unitOfWork.SaveChangesAsync();
        
        var programacaoComInclude = await _programacaoEventoRepository.GetByIdWithIncludesAsync(programacao.Id);
        return _mapper.Map<ProgramacaoEventoResponseDto>(programacaoComInclude);
    }

    public async Task<ProgramacaoEventoResponseDto> AtualizarAsync(ProgramacaoEventoUpdateDto dto)
    {
        var programacaoExistente = await _programacaoEventoRepository.GetByIdAsync(dto.Id);
        if (programacaoExistente == null)
            throw new ExceptionValidation("Programação não encontrada.");

        _mapper.Map(dto, programacaoExistente);
        
        ExecutarValidacao(new ProgramacaoEventoValidation(), programacaoExistente);

        _programacaoEventoRepository.Update(programacaoExistente);
        await _unitOfWork.SaveChangesAsync();
        
        var programacaoAtualizada = await _programacaoEventoRepository.GetByIdWithIncludesAsync(dto.Id);
        return _mapper.Map<ProgramacaoEventoResponseDto>(programacaoAtualizada);
    }

    public async Task RemoverAsync(int id)
    {
        var programacao = await _programacaoEventoRepository.GetByIdAsync(id);
        if (programacao == null)
            throw new ExceptionValidation("Programação não encontrada.");

        _programacaoEventoRepository.Remove(programacao);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<ProgramacaoEventoResponseDto> ObterPorIdAsync(int id)
    {
        var programacao = await _programacaoEventoRepository.GetByIdWithIncludesAsync(id);
        if (programacao == null)
            throw new ExceptionValidation("Programação não encontrada.");
            
        return _mapper.Map<ProgramacaoEventoResponseDto>(programacao);
    }

    public async Task<IEnumerable<ProgramacaoEventoResponseDto>> ObterPorEventoAsync(int idEvento)
    {
        var programacoes = await _programacaoEventoRepository.GetByEventoAsync(idEvento);
        return _mapper.Map<IEnumerable<ProgramacaoEventoResponseDto>>(programacoes);
    }
}
