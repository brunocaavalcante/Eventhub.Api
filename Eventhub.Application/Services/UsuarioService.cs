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

    public UsuarioService(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Usuario> AdicionarAsync(Usuario usuario)
    {
        ExecutarValidacao(new UsuarioValidation(), usuario);

        if (await _usuarioRepository.EmailExistsAsync(usuario.Email))
            throw new ExceptionValidation("E-mail já cadastrado.");

        await _usuarioRepository.AddAsync(usuario);
        await _unitOfWork.CommitTransactionAsync();
        return usuario;
    }

    public async Task<Usuario> AtualizarAsync(Usuario usuario)
    {
        ExecutarValidacao(new UsuarioValidation(), usuario);

        var usuarioExistente = await _usuarioRepository.GetByIdAsync(usuario.Id);
        if (usuarioExistente == null)
            throw new ExceptionValidation("Usuário não encontrado.");

        _usuarioRepository.Update(usuario);
        await _unitOfWork.CommitTransactionAsync();
        return usuario;
    }

    public async Task RemoverAsync(int id)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        if (usuario == null)
            throw new ExceptionValidation("Usuário não encontrado.");

        _usuarioRepository.Remove(usuario);
        await _unitOfWork.CommitTransactionAsync();
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return await _usuarioRepository.GetByEmailAsync(email);
    }
}
