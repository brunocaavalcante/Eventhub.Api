using Eventhub.Application.Interfaces;
using Eventhub.Application.Validations;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class GaleriaService : BaseService, IGaleriaService
{
    private readonly IGaleriaRepository _galeriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GaleriaService(IGaleriaRepository galeriaRepository, IUnitOfWork unitOfWork)
    {
        _galeriaRepository = galeriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Galeria> AdicionarAsync(Galeria galeria)
    {
        ExecutarValidacao(new GaleriaValidation(), galeria);

        await _galeriaRepository.AddAsync(galeria);
        await _unitOfWork.CommitTransactionAsync();
        return galeria;
    }

    public async Task<Galeria> AtualizarAsync(Galeria galeria)
    {
        ExecutarValidacao(new GaleriaValidation(), galeria);

        var galeriaExistente = await _galeriaRepository.GetByIdAsync(galeria.Id);
        if (galeriaExistente == null)
            throw new ExceptionValidation("Galeria não encontrada.");

        _galeriaRepository.Update(galeria);
        await _unitOfWork.CommitTransactionAsync();
        return galeria;
    }

    public async Task RemoverAsync(int id)
    {
        var galeria = await _galeriaRepository.GetByIdAsync(id);
        if (galeria == null)
            throw new ExceptionValidation("Galeria não encontrada.");

        _galeriaRepository.Remove(galeria);
        await _unitOfWork.CommitTransactionAsync();
    }

    public async Task<IEnumerable<Galeria>> ObterPorEventoAsync(int idEvento)
    {
        return await _galeriaRepository.GetByEventoAsync(idEvento);
    }
}
