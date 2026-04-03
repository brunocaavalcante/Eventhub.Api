namespace Eventhub.Application.Interfaces;

public interface IPermissaoEventoService
{
    Task<bool> TemPermissaoAsync(int idUsuario, int idEvento, string chavePermissao);
    Task<Dictionary<string, bool>> ListarPermissoesEventoAsync(int idUsuario, int idEvento);
}
