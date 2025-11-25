using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Validations;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class UsuarioService : BaseService, IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKeycloakService _keycloakService;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork,
        IKeycloakService keycloakService)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _keycloakService = keycloakService;
    }

    public async Task<Usuario> AdicionarAsync(CreateUsuarioDto createUsuarioDto)
    {
        if (string.IsNullOrWhiteSpace(createUsuarioDto.Password))
            throw new ExceptionValidation("Senha é obrigatória.");

        if (await _usuarioRepository.EmailExistsAsync(createUsuarioDto.Email))
            throw new ExceptionValidation("E-mail já cadastrado.");

        var usuario = new Usuario
        {
            Nome = createUsuarioDto.Nome,
            Email = createUsuarioDto.Email,
            Telefone = createUsuarioDto.Telefone,
            DataCadastro = DateTime.UtcNow,
            Status = "Ativo"
        };

        ExecutarValidacao(new UsuarioValidation(), usuario);

        await _usuarioRepository.AddAsync(usuario);

        try
        {
            var keycloakId = await _keycloakService.CriarUsuarioAsync(
                createUsuarioDto.Nome,
                createUsuarioDto.Email,
                createUsuarioDto.Password
            );

            usuario.KeycloakId = keycloakId;

            await _unitOfWork.CommitTransactionAsync();

            return usuario;
        }
        catch (Exception ex) when (ex is not ExceptionValidation)
        {
            throw new ExceptionValidation($"Erro ao criar usuário no Keycloak: {ex.Message}");
        }
    }

    public async Task<Usuario> AtualizarAsync(Usuario usuario)
    {
        ExecutarValidacao(new UsuarioValidation(), usuario);

        var usuarioExistente = await _usuarioRepository.GetByIdAsync(usuario.Id);
        if (usuarioExistente == null)
            throw new ExceptionValidation("Usuário não encontrado.");

        try
        {
            // Atualizar no Keycloak
            await _keycloakService.AtualizarUsuarioAsync(
                usuarioExistente.Email,
                usuario.Nome,
                usuario.Email != usuarioExistente.Email ? usuario.Email : null
            );

            _usuarioRepository.Update(usuario);
            await _unitOfWork.CommitTransactionAsync();

            return usuario;
        }
        catch (Exception ex) when (ex is not ExceptionValidation)
        {
            throw new ExceptionValidation($"Erro ao atualizar usuário: {ex.Message}");
        }
    }

    public async Task RemoverAsync(int id)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        if (usuario == null)
            throw new ExceptionValidation("Usuário não encontrado.");

        try
        {
            await _keycloakService.DeletarUsuarioAsync(usuario.Email);

            _usuarioRepository.Remove(usuario);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex) when (ex is not ExceptionValidation)
        {
            throw new ExceptionValidation($"Erro ao remover usuário: {ex.Message}");
        }
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return await _usuarioRepository.GetByEmailAsync(email);
    }
}
