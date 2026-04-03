using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Validations;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class NotificacaoService : BaseService, INotificacaoService
{
    private readonly INotificacaoRepository _notificacaoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEventoRepository _eventoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificacaoService(
        INotificacaoRepository notificacaoRepository,
        IUsuarioRepository usuarioRepository,
        IEventoRepository eventoRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _notificacaoRepository = notificacaoRepository;
        _usuarioRepository = usuarioRepository;
        _eventoRepository = eventoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<NotificacaoResponseDto> AdicionarAsync(NotificacaoCreateDto dto)
    {
        // Validar se usuários e evento existem
        var usuarioOrigem = await _usuarioRepository.GetByIdAsync(dto.IdUsuarioOrigem);
        if (usuarioOrigem == null)
            throw new ExceptionValidation("Usuário de origem não encontrado.");

        var usuarioDestino = await _usuarioRepository.GetByIdAsync(dto.IdUsuarioDestino);
        if (usuarioDestino == null)
            throw new ExceptionValidation("Usuário de destino não encontrado.");

        var evento = await _eventoRepository.GetByIdAsync(dto.IdEvento);
        if (evento == null)
            throw new ExceptionValidation("Evento não encontrado.");

        var notificacao = _mapper.Map<Notificacao>(dto);
        notificacao.DataCadastro = DateTime.UtcNow;
        notificacao.DataEnvio = DateTime.UtcNow;
        notificacao.DataLeitura = DateTime.MinValue;
        notificacao.Data = DateTime.UtcNow;

        ExecutarValidacao(new NotificacaoValidation(), notificacao);

        await _notificacaoRepository.AddAsync(notificacao);
        await _unitOfWork.SaveChangesAsync();

        // Recarregar com includes para retornar DTO completo
        var notificacaoComIncludes = await _notificacaoRepository.GetByIdWithIncludesAsync(notificacao.Id);
        return _mapper.Map<NotificacaoResponseDto>(notificacaoComIncludes);
    }

    public async Task RemoverAsync(int id, int idUsuario)
    {
        var notificacao = await _notificacaoRepository.GetByIdAsync(id);
        if (notificacao == null)
            throw new ExceptionValidation("Notificação não encontrada.");

        // Validar permissão: usuário só pode deletar suas próprias notificações
        if (notificacao.IdUsuarioDestino != idUsuario)
            throw new ExceptionValidation("Você não tem permissão para remover esta notificação.", true);

        _notificacaoRepository.Remove(notificacao);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<NotificacaoResponseDto?> ObterPorIdAsync(int id, int idUsuario)
    {
        var notificacao = await _notificacaoRepository.GetByIdWithIncludesAsync(id);

        if (notificacao == null)
            return null;

        // Validar permissão: usuário só pode ver suas próprias notificações
        if (notificacao.IdUsuarioDestino != idUsuario)
            throw new ExceptionValidation("Você não tem permissão para acessar esta notificação.", true);

        return _mapper.Map<NotificacaoResponseDto>(notificacao);
    }

    public async Task<IEnumerable<NotificacaoResponseDto>> ObterPorUsuarioAsync(int idUsuario)
    {
        var notificacoes = await _notificacaoRepository.GetByUsuarioDestinoAsync(idUsuario);
        return _mapper.Map<IEnumerable<NotificacaoResponseDto>>(notificacoes);
    }

    public async Task<IEnumerable<NotificacaoResponseDto>> ObterNaoLidasAsync(int idUsuario)
    {
        var notificacoes = await _notificacaoRepository.GetNaoLidasByUsuarioAsync(idUsuario);
        return _mapper.Map<IEnumerable<NotificacaoResponseDto>>(notificacoes);
    }

    public async Task<NotificacaoResponseDto> MarcarComoLidaAsync(int id, int idUsuario)
    {
        var notificacao = await _notificacaoRepository.GetByIdAsync(id);
        if (notificacao == null)
            throw new ExceptionValidation("Notificação não encontrada.");

        // Validar permissão: usuário só pode marcar como lida suas próprias notificações
        if (notificacao.IdUsuarioDestino != idUsuario)
            throw new ExceptionValidation("Você não tem permissão para marcar esta notificação como lida.", true);

        notificacao.Status = EnumNotificacaoStatus.Lida;
        notificacao.DataLeitura = DateTime.UtcNow;

        _notificacaoRepository.Update(notificacao);
        await _unitOfWork.SaveChangesAsync();

        // Recarregar com includes
        var notificacaoAtualizada = await _notificacaoRepository.GetByIdWithIncludesAsync(id);
        return _mapper.Map<NotificacaoResponseDto>(notificacaoAtualizada);
    }

    public async Task MarcarTodasComoLidasAsync(int idUsuario)
    {
        await _notificacaoRepository.MarcarTodasComoLidasAsync(idUsuario);
        await _unitOfWork.SaveChangesAsync();
    }
}
