using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Validations;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class ParticipanteService : BaseService, IParticipanteService
{
    private readonly IParticipanteRepository _participanteRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private const string StatusPendente = "PendenteCadastro";

    public ParticipanteService(
        IParticipanteRepository participanteRepository,
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _participanteRepository = participanteRepository;
        _usuarioRepository = usuarioRepository;
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
        return novoUsuario;
    }

    private static bool UsuarioEstaPendente(Usuario usuario)
        => string.Equals(usuario.Status, StatusPendente, StringComparison.OrdinalIgnoreCase);
}
