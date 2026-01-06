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
    private readonly IAuth0Service _auth0Service;

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
