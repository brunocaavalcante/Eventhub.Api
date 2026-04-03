namespace Eventhub.Application.Helpers;

/// <summary>
/// Helper para gerenciamento de emails temporários gerados para usuários pendentes de cadastro.
/// </summary>
public static class EmailHelper
{
    private const string DOMINIO_EMAIL_TEMPORARIO = "@eventhub.temp";

    /// <summary>
    /// Gera um email temporário único usando GUID para usuários sem email cadastrado.
    /// Formato: pendente_{guid}@eventhub.temp
    /// </summary>
    /// <returns>Email temporário único</returns>
    public static string GerarEmailTemporario()
    {
        var guid = Guid.NewGuid().ToString("N"); // Formato sem hífens
        return $"pendente_{guid}{DOMINIO_EMAIL_TEMPORARIO}";
    }

    /// <summary>
    /// Verifica se um email é temporário (gerado pelo sistema).
    /// </summary>
    /// <param name="email">Email a ser verificado</param>
    /// <returns>True se o email é temporário, False caso contrário</returns>
    public static bool EhEmailTemporario(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return email.EndsWith(DOMINIO_EMAIL_TEMPORARIO, StringComparison.OrdinalIgnoreCase);
    }
}
