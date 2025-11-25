namespace Eventhub.Application.Interfaces;

public interface IKeycloakService
{
    Task<string> CriarUsuarioAsync(string nome, string email, string password);
    Task AtualizarUsuarioAsync(string email, string novoNome, string? novoEmail = null);
    Task DeletarUsuarioAsync(string email);
    Task<string> ObterAccessTokenAdminAsync();
}
