using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Helpers;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Validations;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class ParticipanteService : BaseService, IParticipanteService
{
    private readonly IParticipanteRepository _participanteRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEventoRepository _eventoRepository;
    private readonly IParticipantePermissaoService _participantePermissaoService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private const string StatusPendente = "PendenteCadastro";

    public ParticipanteService(
        IParticipanteRepository participanteRepository,
        IUsuarioRepository usuarioRepository,
        IEventoRepository eventoRepository,
        IParticipantePermissaoService participantePermissaoService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _participanteRepository = participanteRepository;
        _usuarioRepository = usuarioRepository;
        _eventoRepository = eventoRepository;
        _participantePermissaoService = participantePermissaoService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ParticipanteDto> AdicionarAsync(CreateParticipanteDto participanteDto)
    {
        var usuario = await ObterOuCriarUsuarioAsync(participanteDto);

        var participante = new Participante
        {
            IdEvento = participanteDto.IdEvento,
            IdPerfil = participanteDto.IdPerfil,
            Usuario = usuario,
            IdUsuario = usuario.Id,
            CadastroPendente = UsuarioEstaPendente(usuario),
            DataCadastro = DateTime.UtcNow
        };

        if (await _participanteRepository.ExistsAsync(participante.IdEvento, participante.IdUsuario, participante.IdPerfil))
            throw new ExceptionValidation("Participante já vinculado a este evento para o perfil informado.");

        ExecutarValidacao(new ParticipanteValidation(), participante);

        // Configurar permissões individuais se fornecidas
        if (participanteDto.ConfiguracaoVisibilidade != null)
        {
            var permissoes = PermissaoMapper.DtoToPermissoes(participanteDto.ConfiguracaoVisibilidade);
            await _participantePermissaoService.ConfigurarPermissoesParticipanteAsync(participante.Id, permissoes);
        }

        await _participanteRepository.AddAsync(participante);
        await _unitOfWork.SaveChangesAsync();

        var participanteSalvo = await _participanteRepository.GetByIdWithDetailsAsync(participante.Id) ?? participante;
        return _mapper.Map<ParticipanteDto>(participanteSalvo);
    }

    public async Task<ParticipanteDto> AtualizarAsync(UpdateParticipanteDto participanteDto)
    {
        var participante = await _participanteRepository.GetByIdWithDetailsAsync(participanteDto.Id)
            ?? throw new ExceptionValidation("Participante não encontrado.");

        if (participanteDto.IdUsuario.HasValue && participante.IdUsuario != participanteDto.IdUsuario.Value)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(participanteDto.IdUsuario.Value)
                ?? throw new ExceptionValidation("Usuário informado não foi localizado.");
            participante.Usuario = usuario;
            participante.IdUsuario = usuario.Id;
        }

        participante.IdPerfil = participanteDto.IdPerfil;
        participante.CadastroPendente = UsuarioEstaPendente(participante.Usuario);

        if (await _participanteRepository.ExistsAsync(participante.IdEvento, participante.IdUsuario, participante.IdPerfil, participante.Id))
            throw new ExceptionValidation("Participante já vinculado a este evento para o perfil desejado.");

        ExecutarValidacao(new ParticipanteValidation(), participante);

        _participanteRepository.Update(participante);
        await _unitOfWork.CommitTransactionAsync();

        var participanteAtualizado = await _participanteRepository.GetByIdWithDetailsAsync(participante.Id) ?? participante;
        return _mapper.Map<ParticipanteDto>(participanteAtualizado);
    }

    public async Task RemoverAsync(int id)
    {
        var participante = await _participanteRepository.GetByIdAsync(id)
            ?? throw new ExceptionValidation("Participante não encontrado.");

        _participanteRepository.Remove(participante);
        await _unitOfWork.CommitTransactionAsync();
    }

    public async Task<IEnumerable<ListarConvidadoDto>> ObterConvidadosPorEventoAsync(int idEvento)
    {
        var convidados = await _participanteRepository.ObterConvidadoAcompanhantesPorEvento(idEvento);
        return _mapper.Map<IEnumerable<ListarConvidadoDto>>(convidados);
    }

    public async Task<IEnumerable<ParticipanteDto>> ObterPorEventoAsync(int idEvento)
    {
        var participantes = await _participanteRepository.GetByEventoAsync(idEvento);
        return _mapper.Map<IEnumerable<ParticipanteDto>>(participantes);
    }

    public async Task<ParticipanteDto?> ObterPorIdAsync(int id)
    {
        var participante = await _participanteRepository.GetByIdWithDetailsAsync(id);
        return participante == null ? null : _mapper.Map<ParticipanteDto>(participante);
    }

    public async Task<ParticipanteDto?> ObterPorUsuarioEventoAsync(int idParticipante, int idEvento)
    {
        var participante = await _participanteRepository.GetByUsuarioEventoWithDetailsAsync(idParticipante, idEvento);
        return participante == null ? null : _mapper.Map<ParticipanteDto>(participante);
    }

    public async Task<IEnumerable<ListarConvidadoDto>> ObterConfirmadosAsync(int idEvento)
    {
        var confirmados = await _participanteRepository.ObterConfirmadosPorEventoAsync(idEvento);
        return _mapper.Map<IEnumerable<ListarConvidadoDto>>(confirmados);
    }

    public async Task<ParticipanteDto> ConfirmarPresencaAsync(ConfirmarPresencaDto dto)
    {
        var evento = await _eventoRepository.GetByTokenAsync(dto.TokenEvento)
            ?? throw new ExceptionValidation("Evento não encontrado.");

        if (evento.IdStatus == (int)EventoStatus.Cancelado || evento.IdStatus == (int)EventoStatus.Concluido)
            throw new ExceptionValidation("O link deste evento não está mais ativo.");

        var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email)
            ?? throw new ExceptionValidation("Usuário não encontrado. Realize o cadastro antes de confirmar presença.");

        var participante = await _participanteRepository.GetByUsuarioEventoWithDetailsAsync(usuario.Id, evento.Id);
        if (participante == null)
        {
            participante = new Participante
            {
                IdEvento = evento.Id,
                IdPerfil = (int)EnumPerfil.Convidado,
                IdUsuario = usuario.Id,
                Usuario = usuario,
                CadastroPendente = false,
                DataCadastro = DateTime.UtcNow
            };
            await _participanteRepository.AddAsync(participante);
        }

        participante.IdStatusConvite = (int)EnumStatusEnvioConvite.Pendente;
        participante.QtdAcompanhantes = dto.QtdAcompanhantes;
        participante.MensagemOrganizador = dto.MensagemOrganizador;
        participante.MotivoRecusa = null;
        participante.DataResposta = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        var salvo = await _participanteRepository.GetByIdWithDetailsAsync(participante.Id) ?? participante;
        return _mapper.Map<ParticipanteDto>(salvo);
    }

    public async Task<ParticipanteDto> RecusarConviteAsync(RecusarConviteDto dto)
    {
        var evento = await _eventoRepository.GetByTokenAsync(dto.TokenEvento)
            ?? throw new ExceptionValidation("Evento não encontrado.");

        if (evento.IdStatus == (int)EventoStatus.Cancelado || evento.IdStatus == (int)EventoStatus.Concluido)
            throw new ExceptionValidation("O link deste evento não está mais ativo.");

        var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email)
            ?? throw new ExceptionValidation("Usuário não encontrado. Realize o cadastro antes de recusar o convite.");

        var participante = await _participanteRepository.GetByUsuarioEventoWithDetailsAsync(usuario.Id, evento.Id);
        if (participante == null)
        {
            participante = new Participante
            {
                IdEvento = evento.Id,
                IdPerfil = (int)EnumPerfil.Convidado,
                IdUsuario = usuario.Id,
                Usuario = usuario,
                CadastroPendente = false,
                DataCadastro = DateTime.UtcNow
            };
            await _participanteRepository.AddAsync(participante);
        }

        participante.IdStatusConvite = (int)EnumStatusEnvioConvite.Recusado;
        participante.QtdAcompanhantes = 0;
        participante.MotivoRecusa = dto.MotivoRecusa;
        participante.MensagemOrganizador = null;
        participante.DataResposta = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        var salvo = await _participanteRepository.GetByIdWithDetailsAsync(participante.Id) ?? participante;
        return _mapper.Map<ParticipanteDto>(salvo);
    }

    public async Task<ParticipanteDto> AprovarPresencaAsync(int idParticipante, AprovarPresencaDto dto)
    {
        var participante = await _participanteRepository.GetByIdWithDetailsAsync(idParticipante)
            ?? throw new ExceptionValidation("Participante não encontrado.", true);

        if (participante.IdEvento != dto.IdEvento)
            throw new ExceptionValidation("Participante não pertence ao evento informado.",true);

        var evento = await _eventoRepository.GetByIdAsync(dto.IdEvento)
            ?? throw new ExceptionValidation("Evento não encontrado.", true);

        var totalConfirmados = await _participanteRepository.ContarConfirmadosAsync(dto.IdEvento);
        if (totalConfirmados >= evento.MaxConvidado)
            throw new ExceptionValidation("Limite de convidados do evento atingido.", true);

        participante.IdStatusConvite = (int)EnumStatusEnvioConvite.Confirmado;

        _participanteRepository.Update(participante);
        await _unitOfWork.SaveChangesAsync();

        var atualizado = await _participanteRepository.GetByIdWithDetailsAsync(participante.Id) ?? participante;
        return _mapper.Map<ParticipanteDto>(atualizado);
    }

    private async Task<Usuario> ObterOuCriarUsuarioAsync(CreateParticipanteDto participanteDto)
    {
        if (string.IsNullOrWhiteSpace(participanteDto.Nome) || string.IsNullOrWhiteSpace(participanteDto.Email))
            throw new ExceptionValidation("Nome e e-mail são obrigatórios para criar um usuário temporário.");

        var usuarioExistente = await _usuarioRepository.GetByEmailAsync(participanteDto.Email);
        if (usuarioExistente != null)
            return usuarioExistente;

        var novoUsuario = new Usuario
        {
            Nome = participanteDto.Nome,
            Email = participanteDto.Email,
            Telefone = participanteDto.Telefone,
            DataCadastro = DateTime.UtcNow,
            Status = StatusPendente
        };

        ExecutarValidacao(new UsuarioValidation(), novoUsuario);

        await _usuarioRepository.AddAsync(novoUsuario);
        await _unitOfWork.SaveChangesAsync();
        
        return novoUsuario;
    }

    private static bool UsuarioEstaPendente(Usuario usuario)
        => string.Equals(usuario.Status, StatusPendente, StringComparison.OrdinalIgnoreCase);
}
