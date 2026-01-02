namespace Eventhub.Domain.Interfaces;

public interface IPasswordSecurity
{
    /// <summary>
    /// Criptografa uma senha usando AES
    /// </summary>
    /// <param name="password">Senha em texto plano</param>
    /// <returns>Senha criptografada em Base64</returns>
    string EncryptPassword(string password);

    /// <summary>
    /// Descriptografa uma senha criptografada
    /// </summary>
    /// <param name="encryptedPassword">Senha criptografada em Base64</param>
    /// <returns>Senha em texto plano</returns>
    string DecryptPassword(string encryptedPassword);
}
