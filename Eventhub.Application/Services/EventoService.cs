using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Validations;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class EventoService : BaseService, IEventoService
{
    private readonly IFotosService _fotosService;
    private readonly IParticipanteService _participanteService;
    private readonly IEventoRepository _eventoRepository;
    private readonly IStatusEventoRepository _statusEventoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EventoService(IEventoRepository eventoRepository, IUnitOfWork unitOfWork, IMapper mapper,
    IFotosService fotosService,
    IParticipanteService participanteService,
    IStatusEventoRepository statusEventoRepository)
    {
        _eventoRepository = eventoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fotosService = fotosService;
        _participanteService = participanteService;
        _statusEventoRepository = statusEventoRepository;
    }

    public async Task<IEnumerable<EventoAtivoDto>> ObterEventosPorUsuarioAsync(int idUsuario)
    {
        var eventos = await _eventoRepository.GetEventosByUsuarioAsync(idUsuario);
        return _mapper.Map<IEnumerable<EventoAtivoDto>>(eventos);
    }

    public async Task<IEnumerable<StatusEventoDto>> ObterStatusEventosAsync()
    {
        var statusEventos = await _statusEventoRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<StatusEventoDto>>(statusEventos);
    }

    public async Task<EventoCadastroDto> AdicionarAsync(EventoCadastroDto eventoDto)
    {
        await _unitOfWork.BeginTransactionAsync();
        var evento = _mapper.Map<Evento>(eventoDto);

        ExecutarValidacao(new EventoValidation(), evento);
        ExecutarValidacao(new EnderecoEventoValidation(), evento.Endereco);

        evento.DataInclusao = DateTime.UtcNow;
        evento.IdStatus = (int)EventoStatus.Ativo;

        await _eventoRepository.AddAsync(evento);
        await _unitOfWork.SaveChangesAsync();


        foreach (var imageDto in eventoDto.Imagens)
        {
            var fotoDto = await _fotosService.UploadAsync(imageDto);

            var galeria = new Galeria
            {
                IdEvento = evento.Id,
                IdFoto = fotoDto.Id,
                Visibilidade = "Publica",
                Tipo = GaleriaTipo.Local,
                Data = DateTime.UtcNow
            };

            evento.Galerias.Add(galeria);
        }

        foreach (var participanteDto in eventoDto.Participantes)
        {
            participanteDto.IdEvento = evento.Id;
            await _participanteService.AdicionarAsync(participanteDto);
        }

        await _unitOfWork.CommitTransactionAsync();

        return eventoDto;
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
