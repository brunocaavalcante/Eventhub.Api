namespace Eventhub.Application.Interfaces;

public interface IPerfilEventoPermissaoService
{
    Task ConfigurarPermissoesPerfilAsync(int idEvento, int idPerfil, Dictionary<string, bool> permissoes);
    Task<Dictionary<string, bool>> ObterConfiguracaoPerfilAsync(int idEvento, int idPerfil);
    Task RemoverConfiguracaoPerfilAsync(int idEvento, int idPerfil);
}
