using Eventhub.Application.Interfaces;
using Eventhub.Application.Validations;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class EventoService : BaseService, IEventoService
{
    private readonly IEventoRepository _eventoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EventoService(IEventoRepository eventoRepository, IUnitOfWork unitOfWork)
    {
        _eventoRepository = eventoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Evento> AdicionarAsync(Evento evento)
    {
        ExecutarValidacao(new EventoValidation(), evento);
        await _eventoRepository.AddAsync(evento);
        await _unitOfWork.CommitTransactionAsync();
        return evento;
    }

    public async Task<Evento> AtualizarAsync(Evento evento)
    {
        ExecutarValidacao(new EventoValidation(), evento);
        _eventoRepository.Update(evento);
        await _unitOfWork.CommitTransactionAsync();
        return evento;
    }

    public async Task RemoverAsync(int id)
    {
        var evento = await _eventoRepository.GetByIdAsync(id);
        if (evento == null)
            throw new ExceptionValidation("Evento não encontrado.");

        _eventoRepository.Remove(evento);
        await _unitOfWork.CommitTransactionAsync();
    }
}
