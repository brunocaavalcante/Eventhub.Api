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
        // Buscar eventos criados pelo usuário
        var eventosCriados = await _eventoRepository.GetEventosByUsuarioAsync(idUsuario);
        
        // Buscar eventos onde o usuário é participante
        var eventosParticipante = await _eventoRepository.GetEventosByParticipanteUsuarioAsync(idUsuario);
        
        // Combinar e remover duplicados (caso usuário seja criador E participante do mesmo evento)
        var eventosUnificados = eventosCriados
            .Union(eventosParticipante)
            .DistinctBy(e => e.Id)
            .OrderBy(e => e.DataInicio);
        
        return _mapper.Map<IEnumerable<EventoAtivoDto>>(eventosUnificados);
    }

    public async Task<EventoDto?> ObterPorIdAsync(int id)
    {
        var evento = await _eventoRepository.GetByIdAsync(id);
        if (evento == null) return null;

        return _mapper.Map<EventoDto>(evento);
    }

    public async Task<IEnumerable<StatusEventoDto>> ObterStatusEventosAsync()
    {
        var statusEventos = await _statusEventoRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<StatusEventoDto>>(statusEventos);
    }

    public async Task<EventoCadastroDto> AdicionarAsync(EventoCadastroDto eventoDto)
    {
        var evento = _mapper.Map<Evento>(eventoDto);

        ExecutarValidacao(new EventoValidation(), evento);
        ExecutarValidacao(new EnderecoEventoValidation(), evento.Endereco);

        evento.DataInclusao = DateTime.UtcNow;
        evento.IdStatus = (int)EventoStatus.Ativo;
        evento.TokenConvite = Guid.NewGuid();

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

        return eventoDto;
    }

    public async Task<EventoAtivoDto?> ObterPorTokenAsync(Guid token)
    {
        var evento = await _eventoRepository.GetByTokenAsync(token);
        if (evento == null) return null;

        if (evento.IdStatus == (int)EventoStatus.Cancelado || evento.IdStatus == (int)EventoStatus.Concluido)
            return null;

        return _mapper.Map<EventoAtivoDto>(evento);
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
