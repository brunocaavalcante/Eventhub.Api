using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Infra.Security;

/// <summary>
/// Implementação de criptografia de senhas usando AES compatível com Angular/CryptoJS
/// </summary>
public class PasswordSecurity : IPasswordSecurity
{
    private readonly string _encryptionKey;
    private readonly string _encryptionIv;

    public PasswordSecurity(IConfiguration configuration)
    {
        _encryptionKey = configuration["Security:EncryptionKey"] ?? "1234567890123456";
        _encryptionIv = configuration["Security:EncryptionIv"] ?? "1234567890123456";
        
        // Valida se a chave tem o tamanho correto (16 bytes para AES-128)
        if (Encoding.UTF8.GetBytes(_encryptionKey).Length != 16)
        {
            throw new ArgumentException("Chave de criptografia deve ter exatamente 16 caracteres (128 bits)");
        }
        
        // Valida se o IV tem o tamanho correto (16 bytes para AES-128)
        if (Encoding.UTF8.GetBytes(_encryptionIv).Length != 16)
        {
            throw new ArgumentException("IV deve ter exatamente 16 caracteres (128 bits)");
        }
    }

    public string EncryptPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Senha não pode ser nula ou vazia", nameof(password));

        byte[] keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);
        byte[] ivBytes = Encoding.UTF8.GetBytes(_encryptionIv);
        
        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = ivBytes;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        
        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(password);
        }
        
        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public string DecryptPassword(string encryptedPassword)
    {
        if (string.IsNullOrEmpty(encryptedPassword))
            throw new ArgumentException("Senha criptografada não pode ser nula ou vazia", nameof(encryptedPassword));

        try
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);
            byte[] ivBytes = Encoding.UTF8.GetBytes(_encryptionIv);
            byte[] encrypted = Convert.FromBase64String(encryptedPassword);

            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = ivBytes; // Usa IV fixo como no Angular
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var msDecrypt = new MemoryStream(encrypted);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            
            return srDecrypt.ReadToEnd();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erro ao descriptografar senha", ex);
        }
    }
}
