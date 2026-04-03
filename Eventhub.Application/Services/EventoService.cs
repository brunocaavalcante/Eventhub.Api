using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.DTOs.Evento;
using Eventhub.Application.Helpers;
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
    private readonly IPerfilEventoPermissaoService _perfilEventoPermissaoService;
    private readonly IPerfilRepository _perfilRepository;
    private readonly IPresenteRepository _presenteRepository;
    private readonly IContribuicaoPresenteRepository _contribuicaoPresenteRepository;
    private readonly IParticipanteRepository _participanteRepository;
    private readonly INotificacaoRepository _notificacaoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EventoService(IEventoRepository eventoRepository, IUnitOfWork unitOfWork, IMapper mapper,
    IFotosService fotosService,
    IParticipanteService participanteService,
    IStatusEventoRepository statusEventoRepository,
    IPerfilEventoPermissaoService perfilEventoPermissaoService,
    IPerfilRepository perfilRepository,
    IPresenteRepository presenteRepository,
    IContribuicaoPresenteRepository contribuicaoPresenteRepository,
    IParticipanteRepository participanteRepository,
    INotificacaoRepository notificacaoRepository,
    IUsuarioRepository usuarioRepository,
    IEmailService emailService)
    {
        _eventoRepository = eventoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fotosService = fotosService;
        _participanteService = participanteService;
        _statusEventoRepository = statusEventoRepository;
        _perfilEventoPermissaoService = perfilEventoPermissaoService;
        _perfilRepository = perfilRepository;
        _presenteRepository = presenteRepository;
        _contribuicaoPresenteRepository = contribuicaoPresenteRepository;
        _participanteRepository = participanteRepository;
        _notificacaoRepository = notificacaoRepository;
        _usuarioRepository = usuarioRepository;
        _emailService = emailService;
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

        // Configurar permissões de visibilidade para convidados
        if (eventoDto.ConfiguracaoVisibilidade != null)
        {
            var perfisAtivos = await _perfilRepository.GetPerfisAtivosAsync();
            var perfilConvidado = perfisAtivos.FirstOrDefault(p => p.Descricao.Equals("Convidado", StringComparison.OrdinalIgnoreCase));

            if (perfilConvidado != null)
            {
                var permissoes = PermissaoMapper.DtoToPermissoes(eventoDto.ConfiguracaoVisibilidade);
                await _perfilEventoPermissaoService.ConfigurarPermissoesPerfilAsync(evento.Id, perfilConvidado.Id, permissoes);
            }
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

    public async Task<EventoDto> AtualizarAsync(UpdateEventoDto evento)
    {
        ExecutarValidacao(new UpdateEventoValidation(), evento);
        var eventoExistente = await _eventoRepository.GetByIdAsync(evento.Id);

        if (eventoExistente == null)
            throw new ExceptionValidation("Evento não encontrado.");

        _mapper.Map(evento, eventoExistente);

        await _eventoRepository.UpdateAsync(eventoExistente);
        return _mapper.Map<EventoDto>(eventoExistente);
    }

    public async Task RemoverAsync(int id)
    {
        var evento = await _eventoRepository.GetByIdAsync(id);
        if (evento == null)
            throw new ExceptionValidation("Evento não encontrado.");

        if (evento.DataInicio <= DateTime.UtcNow)
        {
            throw new ExceptionValidation(
                "Não é possível excluir um evento que já iniciou. Considere cancelá-lo ao invés de excluí-lo.", 
                true);
        }

        if (evento.IdStatus == (int)EventoStatus.Concluido)
        {
            throw new ExceptionValidation(
                "Não é possível excluir um evento que já foi concluído. O histórico deve ser preservado.", 
                true);
        }

        var presentes = await _presenteRepository.GetByEventIdAsync(id);
        var presentesIds = presentes.Select(p => p.Id).ToList();

        if (presentesIds.Any())
        {
            var contribuicoesConfirmadas = await _contribuicaoPresenteRepository
                .FindAsync(c => presentesIds.Contains(c.IdPresente) &&
                               c.IdStatusContribuicao == (int)StatusContribuicaoEnum.Confirmado);

            if (contribuicoesConfirmadas.Any())
            {
                throw new ExceptionValidation(
                    $"Não é possível excluir o evento. Existem {contribuicoesConfirmadas.Count()} contribuição(ões) confirmada(s) que precisam ser estornadas antes da exclusão.", 
                    true);
            }

            var contribuicoesEmAnalise = await _contribuicaoPresenteRepository
                .FindAsync(c => presentesIds.Contains(c.IdPresente) &&
                               c.IdStatusContribuicao == (int)StatusContribuicaoEnum.EmAnalise);

            if (contribuicoesEmAnalise.Any())
            {
                throw new ExceptionValidation(
                    $"Não é possível excluir o evento. Existem {contribuicoesEmAnalise.Count()} contribuição(ões) em análise que precisam ser resolvidas antes da exclusão.", 
                    true);
            }

            // AÇÃO: Cancelar contribuições pendentes automaticamente
            var contribuicoesPendentes = await _contribuicaoPresenteRepository
                .FindAsync(c => presentesIds.Contains(c.IdPresente) &&
                               c.IdStatusContribuicao == (int)StatusContribuicaoEnum.Pendente);

            foreach (var contribuicao in contribuicoesPendentes)
            {
                contribuicao.IdStatusContribuicao = (int)StatusContribuicaoEnum.Cancelado;
                contribuicao.Justificativa = "Contribuição cancelada automaticamente devido à exclusão do evento.";
                await _contribuicaoPresenteRepository.UpdateAsync(contribuicao);
            }
        }

        // REGRA 4: Notificar todos os participantes antes da exclusão
        var participantes = await _participanteRepository.GetByEventoAsync(id);
        
        if (participantes.Any())
        {
            foreach (var participante in participantes)
            {
                var notificacao = new Notificacao
                {
                    IdEvento = evento.Id,
                    IdUsuarioOrigem = evento.IdUsuarioCriador,
                    IdUsuarioDestino = participante.IdUsuario,
                    Data = DateTime.UtcNow,
                    Titulo = "Evento Excluído",
                    Descricao = $"O evento '{evento.Nome}' foi excluído pelo organizador.",
                    LinkAcao = string.Empty,
                    Icone = "delete",
                    Status = EnumNotificacaoStatus.Enviada,
                    Prioridade = EnumNotificacaoPrioridade.Alta,
                    DataCadastro = DateTime.UtcNow,
                    DataEnvio = DateTime.UtcNow
                };

                await _notificacaoRepository.AddAsync(notificacao);
            }

            await _unitOfWork.SaveChangesAsync();

            // Enviar emails para os participantes
            foreach (var participante in participantes)
            {
                var usuario = await _usuarioRepository.GetByIdAsync(participante.IdUsuario);
                if (usuario != null && !string.IsNullOrWhiteSpace(usuario.Email) && !EmailHelper.EhEmailTemporario(usuario.Email))
                {
                    await _emailService.EnviarEmailEventoExcluidoAsync(
                        usuario.Email, 
                        usuario.Nome, 
                        evento.Nome, 
                        evento.DataInicio);
                }
            }
        }
        
        _eventoRepository.Remove(evento);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CancelarEventoAsync(int id, CancelarEventoDto dto)
    {
        // Validar justificativa
        ExecutarValidacao(new CancelarEventoValidation(), dto);

        var evento = await _eventoRepository.GetByIdAsync(id);
        if (evento == null)
            throw new ExceptionValidation("Evento não encontrado.");

        if (evento.IdStatus == (int)EventoStatus.Cancelado)
            throw new ExceptionValidation("O evento já está cancelado.", true);

        if (evento.IdStatus == (int)EventoStatus.Concluido)
            throw new ExceptionValidation("Não é possível cancelar um evento que já foi concluído.", true);

        // Buscar presentes do evento com suas contribuições
        var presentes = await _presenteRepository.GetByEventIdAsync(id);
        var presentesIds = presentes.Select(p => p.Id).ToList();

        // Se não houver presentes, não há contribuições a verificar
        if (presentesIds.Any())
        {
            // Verificar contribuições confirmadas
            var contribuicoesConfirmadas = await _contribuicaoPresenteRepository
                .FindAsync(c => presentesIds.Contains(c.IdPresente) &&
                               c.IdStatusContribuicao == (int)StatusContribuicaoEnum.Confirmado);

            if (contribuicoesConfirmadas.Any())
            {
                throw new ExceptionValidation(
                    $"Não é possível cancelar o evento. Existem {contribuicoesConfirmadas.Count()} contribuição(ões) confirmada(s) que precisam ser estornadas primeiro.", true);
            }

            // Verificar contribuições em análise
            var contribuicoesEmAnalise = await _contribuicaoPresenteRepository
                .FindAsync(c => presentesIds.Contains(c.IdPresente) &&
                               c.IdStatusContribuicao == (int)StatusContribuicaoEnum.EmAnalise);

            if (contribuicoesEmAnalise.Any())
            {
                throw new ExceptionValidation(
                    $"Não é possível cancelar o evento. Existem {contribuicoesEmAnalise.Count()} contribuição(ões) em análise que precisam ser resolvidas primeiro.", true);
            }

            // Cancelar contribuições pendentes
            var contribuicoesPendentes = await _contribuicaoPresenteRepository
                .FindAsync(c => presentesIds.Contains(c.IdPresente) &&
                               c.IdStatusContribuicao == (int)StatusContribuicaoEnum.Pendente);

            foreach (var contribuicao in contribuicoesPendentes)
            {
                contribuicao.IdStatusContribuicao = (int)StatusContribuicaoEnum.Cancelado;
                contribuicao.Justificativa = "Contribuição cancelada automaticamente devido ao cancelamento do evento.";
                await _contribuicaoPresenteRepository.UpdateAsync(contribuicao);
            }

            // Liberar reservas de presentes
            var presentesReservados = presentes.Where(p => p.IdStatus == (int)StatusPresenteEnum.Reservado);
            foreach (var presente in presentesReservados)
            {
                presente.IdStatus = (int)StatusPresenteEnum.Disponivel;
                presente.IdParticipanteReservou = null;
                presente.DataReserva = null;
                await _presenteRepository.UpdateAsync(presente);
            }
        }

        // Atualizar status do evento
        evento.IdStatus = (int)EventoStatus.Cancelado;
        await _eventoRepository.UpdateAsync(evento);

        // Enviar notificações para todos os participantes
        var participantes = await _participanteRepository.GetByEventoAsync(id);
        foreach (var participante in participantes)
        {
            var notificacao = new Notificacao
            {
                IdEvento = evento.Id,
                IdUsuarioOrigem = evento.IdUsuarioCriador,
                IdUsuarioDestino = participante.IdUsuario,
                Data = DateTime.UtcNow,
                Titulo = "Evento Cancelado",
                Descricao = $"O evento '{evento.Nome}' foi cancelado. Motivo: {dto.Justificativa}",
                LinkAcao = $"/eventos/{evento.Id}",
                Icone = "cancel",
                Status = EnumNotificacaoStatus.Enviada,
                Prioridade = EnumNotificacaoPrioridade.Alta,
                DataCadastro = DateTime.UtcNow,
                DataEnvio = DateTime.UtcNow
            };

            await _notificacaoRepository.AddAsync(notificacao);
        }

        await _unitOfWork.SaveChangesAsync();

        // Enviar emails para os participantes
        foreach (var participante in participantes)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(participante.IdUsuario);
            if (usuario != null && !string.IsNullOrWhiteSpace(usuario.Email) && !EmailHelper.EhEmailTemporario(usuario.Email))
            {
                await _emailService.EnviarEmailEventoCanceladoAsync(usuario.Email, usuario.Nome, evento.Nome, dto.Justificativa);
            }
        }
    }

    public async Task ReativarEventoAsync(int id)
    {
        var evento = await _eventoRepository.GetByIdAsync(id);
        if (evento == null)
            throw new ExceptionValidation("Evento não encontrado.");

        // Validação: Apenas eventos cancelados podem ser reativados
        if (evento.IdStatus != (int)EventoStatus.Cancelado)
            throw new ExceptionValidation("Apenas eventos cancelados podem ser reativados.");

        // Validação: Evento não pode ter terminado
        if (evento.DataFim < DateTime.UtcNow)
            throw new ExceptionValidation("Não é possível reativar um evento que já foi concluído.");

        // Buscar presentes do evento com suas contribuições
        var presentes = await _presenteRepository.GetByEventIdAsync(id);
        var presentesIds = presentes.Select(p => p.Id).ToList();

        // Se houver presentes, verificar contribuições estornadas
        if (presentesIds.Any())
        {
            var contribuicoesEstornadas = await _contribuicaoPresenteRepository
                .FindAsync(c => presentesIds.Contains(c.IdPresente) &&
                               c.IdStatusContribuicao == (int)StatusContribuicaoEnum.Estornado);

            if (contribuicoesEstornadas.Any())
            {
                throw new ExceptionValidation(
                    $"Não é possível reativar o evento. Existem {contribuicoesEstornadas.Count()} contribuição(ões) já estornada(s). Questões financeiras já foram resolvidas.");
            }

            // Enviar emails para os participantes
            var participantes = await _participanteRepository.GetByEventoAsync(id);
            foreach (var participante in participantes)
            {
                var usuario = await _usuarioRepository.GetByIdAsync(participante.IdUsuario);
                if (usuario != null && !string.IsNullOrWhiteSpace(usuario.Email) && !EmailHelper.EhEmailTemporario(usuario.Email))
                {
                    await _emailService.EnviarEmailEventoReativadoAsync(usuario.Email, usuario.Nome, evento.Nome);
                }
            }
        }

        // Determinar o novo status baseado na data
        var novoStatus = evento.DataInicio > DateTime.UtcNow
            ? (int)EventoStatus.Agendado
            : (int)EventoStatus.Ativo;

        // Atualizar status do evento
        evento.IdStatus = novoStatus;
        await _eventoRepository.UpdateAsync(evento);
        await _unitOfWork.SaveChangesAsync();
    }
}