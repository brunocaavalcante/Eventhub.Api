using Eventhub.Application.Interfaces;
using Eventhub.Application.Validations;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class NotificacaoService : BaseService, INotificacaoService
{
    private readonly INotificacaoRepository _notificacaoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NotificacaoService(INotificacaoRepository notificacaoRepository, IUnitOfWork unitOfWork)
    {
        _notificacaoRepository = notificacaoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Notificacao> AdicionarAsync(Notificacao notificacao)
    {
        ExecutarValidacao(new NotificacaoValidation(), notificacao);

        await _notificacaoRepository.AddAsync(notificacao);
        await _unitOfWork.CommitTransactionAsync();
        return notificacao;
    }

    public async Task<Notificacao> AtualizarAsync(Notificacao notificacao)
    {
        ExecutarValidacao(new NotificacaoValidation(), notificacao);

        var notificacaoExistente = await _notificacaoRepository.GetByIdAsync(notificacao.Id);
        if (notificacaoExistente == null)
            throw new ExceptionValidation("Notificação não encontrada.");

        _notificacaoRepository.Update(notificacao);
        await _unitOfWork.CommitTransactionAsync();
        return notificacao;
    }

    public async Task RemoverAsync(int id)
    {
        var notificacao = await _notificacaoRepository.GetByIdAsync(id);
        if (notificacao == null)
            throw new ExceptionValidation("Notificação não encontrada.");

        _notificacaoRepository.Remove(notificacao);
        await _unitOfWork.CommitTransactionAsync();
    }

    public async Task<IEnumerable<Notificacao>> ObterPorUsuarioAsync(int idUsuario)
    {
        return await _notificacaoRepository.GetByUsuarioDestinoAsync(idUsuario);
    }

    public async Task<IEnumerable<Notificacao>> ObterNaoLidasAsync(int idUsuario)
    {
        return await _notificacaoRepository.GetNaoLidasByUsuarioAsync(idUsuario);
    }
}
