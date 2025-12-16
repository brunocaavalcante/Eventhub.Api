using Eventhub.Application.Interfaces;
using Eventhub.Application.Validations;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
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

        await ValidarFotoCapaUnicaAsync(galeria);

        await _galeriaRepository.AddAsync(galeria);
        await _unitOfWork.SaveChangesAsync();
        return galeria;
    }

    public async Task<Galeria> AtualizarAsync(Galeria galeria)
    {
        ExecutarValidacao(new GaleriaValidation(), galeria);

        var galeriaExistente = await _galeriaRepository.GetByIdAsync(galeria.Id);
        if (galeriaExistente == null)
            throw new ExceptionValidation("Galeria não encontrada.");

        await ValidarFotoCapaUnicaAsync(galeria);

        _galeriaRepository.Update(galeria);
        await _unitOfWork.SaveChangesAsync();
        return galeria;
    }

    public async Task RemoverAsync(int id)
    {
        var galeria = await _galeriaRepository.GetByIdAsync(id);
        if (galeria == null)
            throw new ExceptionValidation("Galeria não encontrada.");

        _galeriaRepository.Remove(galeria);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<Galeria>> ObterPorEventoAsync(int idEvento)
    {
        return await _galeriaRepository.GetByEventoAsync(idEvento);
    }

    private async Task ValidarFotoCapaUnicaAsync(Galeria galeria)
    {
        if (galeria.Tipo != GaleriaTipo.Capa)
            return;

        var existeCapa = await _galeriaRepository.ExistsByTipoAsync(galeria.IdEvento, GaleriaTipo.Capa, galeria.Id == 0 ? null : galeria.Id);
        if (existeCapa)
            throw new ExceptionValidation("O evento já possui uma foto de capa.");
    }
}
