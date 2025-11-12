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

    public ProgramacaoEventoService(IProgramacaoEventoRepository programacaoEventoRepository, IUnitOfWork unitOfWork)
    {
        _programacaoEventoRepository = programacaoEventoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProgramacaoEvento> AdicionarAsync(ProgramacaoEvento programacao)
    {
        ExecutarValidacao(new ProgramacaoEventoValidation(), programacao);

        await _programacaoEventoRepository.AddAsync(programacao);
        await _unitOfWork.CommitTransactionAsync();
        return programacao;
    }

    public async Task<ProgramacaoEvento> AtualizarAsync(ProgramacaoEvento programacao)
    {
        ExecutarValidacao(new ProgramacaoEventoValidation(), programacao);

        var programacaoExistente = await _programacaoEventoRepository.GetByIdAsync(programacao.Id);
        if (programacaoExistente == null)
            throw new ExceptionValidation("Programação não encontrada.");

        _programacaoEventoRepository.Update(programacao);
        await _unitOfWork.CommitTransactionAsync();
        return programacao;
    }

    public async Task RemoverAsync(int id)
    {
        var programacao = await _programacaoEventoRepository.GetByIdAsync(id);
        if (programacao == null)
            throw new ExceptionValidation("Programação não encontrada.");

        _programacaoEventoRepository.Remove(programacao);
        await _unitOfWork.CommitTransactionAsync();
    }

    public async Task<IEnumerable<ProgramacaoEvento>> ObterPorEventoAsync(int idEvento)
    {
        return await _programacaoEventoRepository.GetByEventoAsync(idEvento);
    }
}
