using Eventhub.Application.DTOs;
using Eventhub.Application.Helpers;
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
    private readonly IAuth0Service _auth0Service;

    private const string StatusPendente = "PendenteCadastro";
    private const string StatusAtivo = "Ativo";

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork,
        IAuth0Service auth0Service)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _auth0Service = auth0Service;
    }

    public async Task<Usuario> AdicionarAsync(CreateUsuarioDto createUsuarioDto)
    {
        if (string.IsNullOrWhiteSpace(createUsuarioDto.Password))
            throw new ExceptionValidation("Senha é obrigatória.");

        // Buscar por email OU telefone para detectar usuários temporários ou duplicatas
        var usuarioExistente = await _usuarioRepository.GetByEmailTelefoneAsync(
            createUsuarioDto.Email, 
            createUsuarioDto.Telefone
        );

        // Se encontrou usuário com cadastro pendente (temporário), atualizar ao invés de criar novo
        if (usuarioExistente != null && usuarioExistente.Status == StatusPendente)
        {
            return await AtualizarUsuarioTemporarioAsync(usuarioExistente, createUsuarioDto);
        }

        // Se encontrou usuário ativo, bloquear duplicata
        if (usuarioExistente != null && usuarioExistente.Status == StatusAtivo)
        {
            throw new ExceptionValidation("E-mail ou telefone já cadastrado.");
        }

        // Criar novo usuário normalmente
        var usuario = new Usuario
        {
            Nome = createUsuarioDto.Nome,
            Email = createUsuarioDto.Email,
            Telefone = createUsuarioDto.Telefone,
            DataCadastro = DateTime.UtcNow,
            Status = StatusAtivo
        };

        ExecutarValidacao(new UsuarioValidation(), usuario);

        await _usuarioRepository.AddAsync(usuario);

        try
        {
            var auth0Id = await _auth0Service.CriarUsuarioAsync(
                createUsuarioDto.Nome,
                createUsuarioDto.Email,
                createUsuarioDto.Password
            );

            usuario.KeycloakId = auth0Id;

            await _unitOfWork.CommitTransactionAsync();

            return usuario;
        }
        catch (Exception ex) when (ex is not ExceptionValidation)
        {
            throw new ExceptionValidation($"Erro ao criar usuário no Auth0: {ex.Message}");
        }
    }

    /// <summary>
    /// Atualiza um usuário temporário (Status=PendenteCadastro) para usuário completo.
    /// Utilizado quando organizador adicionou convidado apenas com telefone e o convidado completa cadastro.
    /// </summary>
    private async Task<Usuario> AtualizarUsuarioTemporarioAsync(Usuario usuarioTemporario, CreateUsuarioDto createUsuarioDto)
    {
        // Atualizar dados do usuário temporário com informações reais
        usuarioTemporario.Email = createUsuarioDto.Email;
        usuarioTemporario.Nome = createUsuarioDto.Nome;
        usuarioTemporario.Telefone = createUsuarioDto.Telefone ?? usuarioTemporario.Telefone;
        usuarioTemporario.Status = StatusAtivo;

        ExecutarValidacao(new UsuarioValidation(), usuarioTemporario);

        try
        {
            // Criar conta Auth0 para o usuário
            var auth0Id = await _auth0Service.CriarUsuarioAsync(
                usuarioTemporario.Nome,
                usuarioTemporario.Email,
                createUsuarioDto.Password
            );

            usuarioTemporario.KeycloakId = auth0Id;

            _usuarioRepository.Update(usuarioTemporario);
            await _unitOfWork.CommitTransactionAsync();

            return usuarioTemporario;
        }
        catch (Exception ex) when (ex is not ExceptionValidation)
        {
            throw new ExceptionValidation($"Erro ao criar usuário no Auth0: {ex.Message}");
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
            await _auth0Service.AtualizarUsuarioAsync(
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
            await _auth0Service.DeletarUsuarioAsync(usuario.Email);

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
