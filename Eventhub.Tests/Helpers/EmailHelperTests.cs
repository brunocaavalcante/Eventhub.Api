using Eventhub.Application.Helpers;
using FluentAssertions;

namespace Eventhub.Tests.Helpers;

public class EmailHelperTests
{
    [Fact]
    public void GerarEmailTemporario_DeveRetornarEmailComFormatoCorreto()
    {
        // Act
        var emailTemporario = EmailHelper.GerarEmailTemporario();

        // Assert
        emailTemporario.Should().NotBeNullOrWhiteSpace();
        emailTemporario.Should().StartWith("pendente_");
        emailTemporario.Should().EndWith("@eventhub.temp");
        emailTemporario.Length.Should().BeGreaterThan(20); // pendente_ + GUID(32) + @eventhub.temp
    }

    [Fact]
    public void GerarEmailTemporario_DeveGerarEmailsUnicos()
    {
        // Act
        var email1 = EmailHelper.GerarEmailTemporario();
        var email2 = EmailHelper.GerarEmailTemporario();
        var email3 = EmailHelper.GerarEmailTemporario();

        // Assert
        email1.Should().NotBe(email2);
        email2.Should().NotBe(email3);
        email1.Should().NotBe(email3);
    }

    [Theory]
    [InlineData("pendente_abc123def456@eventhub.temp", true)]
    [InlineData("pendente_12345678901234567890123456789012@eventhub.temp", true)]
    [InlineData("usuario@gmail.com", false)]
    [InlineData("contato@empresa.com.br", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void EhEmailTemporario_DeveIdentificarCorretamente(string? email, bool expectedResult)
    {
        // Act
        var resultado = EmailHelper.EhEmailTemporario(email);

        // Assert
        resultado.Should().Be(expectedResult);
    }

    [Fact]
    public void EhEmailTemporario_DeveSerCaseInsensitive()
    {
        // Arrange
        var emailUpperCase = "PENDENTE_ABC@EVENTHUB.TEMP";
        var emailMixedCase = "Pendente_ABC@EventHub.Temp";

        // Act
        var resultadoUpperCase = EmailHelper.EhEmailTemporario(emailUpperCase);
        var resultadoMixedCase = EmailHelper.EhEmailTemporario(emailMixedCase);

        // Assert
        resultadoUpperCase.Should().BeTrue();
        resultadoMixedCase.Should().BeTrue();
    }

    [Fact]
    public void EhEmailTemporario_DeveFalharParaEmailsComDominioSemelhante()
    {
        // Arrange
        var emailFalso1 = "usuario@eventhub.com";
        var emailFalso2 = "pendente@eventhub.com.temp";
        var emailFalso3 = "test@temp.eventhub";

        // Act & Assert
        EmailHelper.EhEmailTemporario(emailFalso1).Should().BeFalse();
        EmailHelper.EhEmailTemporario(emailFalso2).Should().BeFalse();
        EmailHelper.EhEmailTemporario(emailFalso3).Should().BeFalse();
    }

    [Fact]
    public void GerarEmailTemporario_EmailGeradoDeveSerReconhecidoComoTemporario()
    {
        // Act
        var emailGerado = EmailHelper.GerarEmailTemporario();
        var ehTemporario = EmailHelper.EhEmailTemporario(emailGerado);

        // Assert
        ehTemporario.Should().BeTrue();
    }
}
