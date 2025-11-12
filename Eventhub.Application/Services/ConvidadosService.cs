using Eventhub.Application.Interfaces;
using Eventhub.Application.Validations;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class ConvidadosService : BaseService, IConvidadosService
{
    private readonly IConvidadosRepository _convidadosRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConvidadosService(IConvidadosRepository convidadosRepository, IUnitOfWork unitOfWork)
    {
        _convidadosRepository = convidadosRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Convidados> AdicionarAsync(Convidados convidado)
    {
        ExecutarValidacao(new ConvidadosValidation(), convidado);

        await _convidadosRepository.AddAsync(convidado);
        await _unitOfWork.CommitTransactionAsync();
        return convidado;
    }

    public async Task<Convidados> AtualizarAsync(Convidados convidado)
    {
        ExecutarValidacao(new ConvidadosValidation(), convidado);

        var convidadoExistente = await _convidadosRepository.GetByIdAsync(convidado.Id);
        if (convidadoExistente == null)
            throw new ExceptionValidation("Convidado não encontrado.");

        _convidadosRepository.Update(convidado);
        await _unitOfWork.CommitTransactionAsync();
        return convidado;
    }

    public async Task RemoverAsync(int id)
    {
        var convidado = await _convidadosRepository.GetByIdAsync(id);
        if (convidado == null)
            throw new ExceptionValidation("Convidado não encontrado.");

        _convidadosRepository.Remove(convidado);
        await _unitOfWork.CommitTransactionAsync();
    }

    public async Task<IEnumerable<Convidados>> ObterPorEventoAsync(int idEvento)
    {
        return await _convidadosRepository.GetByEventoAsync(idEvento);
    }
}
