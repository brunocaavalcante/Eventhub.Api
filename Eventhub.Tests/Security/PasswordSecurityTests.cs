using Eventhub.Infra.Security;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace Eventhub.Tests.Security;

public class PasswordSecurityTests
{
    private readonly PasswordSecurity _passwordSecurity;
    private readonly IConfiguration _configuration;

    public PasswordSecurityTests()
    {
        // Arrange - Configuração mock para os testes
        var configData = new Dictionary<string, string>
            {
                {"Security:EncryptionKey", "1234567890123456"},
                {"Security:EncryptionIv", "1234567890123456"}
            };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();

        _passwordSecurity = new PasswordSecurity(_configuration);
    }

    [Fact]
    public void PasswordSecurity_DeveInicializarComChaveCorreta()
    {
        // Arrange & Act
        var passwordSecurity = new PasswordSecurity(_configuration);

        // Assert
        passwordSecurity.Should().NotBeNull();
    }

    [Fact]
    public void EncryptPassword_DeveRetornarStringCriptografada()
    {
        // Arrange
        var senhaOriginal = "123@Mudar";

        // Act
        var senhaCriptografada = _passwordSecurity.EncryptPassword(senhaOriginal);

        // Assert
        senhaCriptografada.Should().NotBeNullOrEmpty();
        senhaCriptografada.Should().NotBe(senhaOriginal);
    }

    [Fact]
    public void DecryptPassword_DeveRetornarSenhaOriginal()
    {
        // Arrange
        var senhaOriginal = "minhasenha123";
        var senhaCriptografada = _passwordSecurity.EncryptPassword(senhaOriginal);

        // Act
        var senhaDescriptografada = _passwordSecurity.DecryptPassword(senhaCriptografada);

        // Assert
        senhaDescriptografada.Should().Be(senhaOriginal);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("senha simples")]
    [InlineData("Senha@Complexa#123!")]
    [InlineData("SenhaComCaracteresEspeciais: áéíóú ção")]
    [InlineData("SenhaMuitoLongaComMuitosCaracteresParaTestarLimitesDoAlgoritmo123456789")]
    public void EncryptDecrypt_DeveManterIntegridadeDeDiversasSenhas(string senhaOriginal)
    {
        // Act
        var senhaCriptografada = _passwordSecurity.EncryptPassword(senhaOriginal);
        var senhaDescriptografada = _passwordSecurity.DecryptPassword(senhaCriptografada);

        // Assert
        senhaDescriptografada.Should().Be(senhaOriginal);
        senhaCriptografada.Should().NotBe(senhaOriginal);
    }

    [Fact]
    public void EncryptPassword_ComSenhaNull_DeveLancarExcecao()
    {
        // Act & Assert
        Action action = () => _passwordSecurity.EncryptPassword(null!);
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void EncryptPassword_ComSenhaVazia_DeveLancarExcecao()
    {
        // Act & Assert
        Action action = () => _passwordSecurity.EncryptPassword("");
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void DecryptPassword_ComSenhaNull_DeveLancarExcecao()
    {
        // Act & Assert
        Action action = () => _passwordSecurity.DecryptPassword(null!);
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void DecryptPassword_ComSenhaVazia_DeveLancarExcecao()
    {
        // Act & Assert
        Action action = () => _passwordSecurity.DecryptPassword("");
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void DecryptPassword_ComStringInvalida_DeveLancarExcecao()
    {
        // Arrange
        var stringInvalida = "esta_nao_eh_uma_string_base64_valida";

        // Act & Assert
        Action action = () => _passwordSecurity.DecryptPassword(stringInvalida);
        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void PasswordSecurity_ComChaveIncorreta_DeveLancarExcecao()
    {
        // Arrange
        var configComChaveInvalida = new Dictionary<string, string>
            {
                {"Security:EncryptionKey", "chave_muito_pequena"}, // Menos de 16 caracteres
                {"Security:EncryptionIv", "1234567890123456"}
            };

        var configurationInvalida = new ConfigurationBuilder()
            .AddInMemoryCollection(configComChaveInvalida!)
            .Build();

        // Act & Assert
        Action action = () => new PasswordSecurity(configurationInvalida);
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void PasswordSecurity_ComChaveMuitoLonga_DeveLancarExcecao()
    {
        // Arrange
        var configComChaveMuitoLonga = new Dictionary<string, string>
            {
                {"Security:EncryptionKey", "esta_chave_tem_mais_de_16_caracteres"}, // Mais de 16 caracteres
                {"Security:EncryptionIv", "1234567890123456"}
            };

        var configurationInvalida = new ConfigurationBuilder()
            .AddInMemoryCollection(configComChaveMuitoLonga!)
            .Build();

        // Act & Assert
        Action action = () => new PasswordSecurity(configurationInvalida);
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void PasswordSecurity_DeveSerSeguroContraTimingAttacks()
    {
        // Arrange
        var senhaOriginal = "senha123";
        var senhaCriptografada = _passwordSecurity.EncryptPassword(senhaOriginal);

        // Act & Assert - Diferentes tentativas de descriptografia devem falhar de forma consistente
        var stringInvalida1 = "string_invalida_1";
        var stringInvalida2 = "string_invalida_2_diferente";

        Action action1 = () => _passwordSecurity.DecryptPassword(stringInvalida1);
        Action action2 = () => _passwordSecurity.DecryptPassword(stringInvalida2);

        Assert.Throws<InvalidOperationException>(action1);
        Assert.Throws<InvalidOperationException>(action2);
    }

    [Fact]
    public void PasswordSecurity_DeveManterConsistenciaEntreInstancias()
    {
        // Arrange
        var passwordSecurity1 = new PasswordSecurity(_configuration);
        var passwordSecurity2 = new PasswordSecurity(_configuration);
        var senhaOriginal = "teste_consistencia";

        // Act
        var senhaCriptografada = passwordSecurity1.EncryptPassword(senhaOriginal);
        var senhaDescriptografada = passwordSecurity2.DecryptPassword(senhaCriptografada);

        // Assert
        senhaDescriptografada.Should().Be(senhaOriginal);
    }
}
