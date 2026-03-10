namespace Eventhub.Application.Interfaces;

public interface IParticipantePermissaoService
{
    Task ConfigurarPermissoesParticipanteAsync(int idParticipante, Dictionary<string, bool> permissoes);
    Task<Dictionary<string, bool>> ObterPermissoesParticipanteAsync(int idParticipante);
    Task RemoverPermissoesParticipanteAsync(int idParticipante);
}
