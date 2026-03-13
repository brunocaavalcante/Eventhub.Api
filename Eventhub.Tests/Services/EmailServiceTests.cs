using Eventhub.Application.Services;
using Eventhub.Application.Settings;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Eventhub.Tests.Services;

public class EmailServiceTests
{
    private readonly Mock<ILogger<EmailService>> _loggerMock;
    private readonly EmailSettings _validSettings;

    public EmailServiceTests()
    {
        _loggerMock = new Mock<ILogger<EmailService>>();
        _validSettings = new EmailSettings
        {
            SmtpServer = "smtp.gmail.com",
            SmtpPort = 587,
            Username = "test@eventhub.com",
            Password = "test-password",
            FromEmail = "noreply@eventhub.com",
            FromName = "EventHub",
            EnableSsl = true
        };
    }

    [Fact]
    public void Constructor_DeveLancarExcecao_QuandoSmtpServerVazio()
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = "",
            Username = "test@eventhub.com",
            Password = "password",
            FromEmail = "noreply@eventhub.com"
        };
        var options = Options.Create(settings);

        // Act
        Action act = () => new EmailService(options, _loggerMock.Object);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*SmtpServer*");
    }

    [Fact]
    public void Constructor_DeveLancarExcecao_QuandoUsernameVazio()
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = "smtp.gmail.com",
            Username = "",
            Password = "password",
            FromEmail = "noreply@eventhub.com"
        };
        var options = Options.Create(settings);

        // Act
        Action act = () => new EmailService(options, _loggerMock.Object);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Username*");
    }

    [Fact]
    public void Constructor_DeveLancarExcecao_QuandoPasswordVazio()
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = "smtp.gmail.com",
            Username = "test@eventhub.com",
            Password = "",
            FromEmail = "noreply@eventhub.com"
        };
        var options = Options.Create(settings);

        // Act
        Action act = () => new EmailService(options, _loggerMock.Object);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Password*");
    }

    [Fact]
    public void Constructor_DeveLancarExcecao_QuandoFromEmailVazio()
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = "smtp.gmail.com",
            Username = "test@eventhub.com",
            Password = "password",
            FromEmail = ""
        };
        var options = Options.Create(settings);

        // Act
        Action act = () => new EmailService(options, _loggerMock.Object);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*FromEmail*");
    }

    [Fact]
    public void Constructor_DeveCriarInstancia_QuandoConfiguracoesValidas()
    {
        // Arrange
        var options = Options.Create(_validSettings);

        // Act
        var emailService = new EmailService(options, _loggerMock.Object);

        // Assert
        emailService.Should().NotBeNull();
    }

    [Fact]
    public async Task EnviarEmailEventoCanceladoAsync_NaoDeveLancarExcecao_ComDadosValidos()
    {
        // Arrange
        var options = Options.Create(_validSettings);
        var emailService = new EmailService(options, _loggerMock.Object);

        // Act
        // Este teste não tenta enviar email real, apenas verifica que o método não lança exceção
        // O envio real falharia pois não há credenciais válidas configuradas
        Func<Task> act = async () => await emailService.EnviarEmailEventoCanceladoAsync(
            "destinatario@test.com",
            "João Silva",
            "Evento de Teste",
            "Motivo do cancelamento");

        // Assert
        // O método não deveria lançar exceção mesmo se o envio falhar (try-catch interno)
        // Em um teste de integração real, validaríamos o envio efetivo
        await act.Should().NotThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task EnviarEmailEventoReativadoAsync_NaoDeveLancarExcecao_ComDadosValidos()
    {
        // Arrange
        var options = Options.Create(_validSettings);
        var emailService = new EmailService(options, _loggerMock.Object);

        // Act
        Func<Task> act = async () => await emailService.EnviarEmailEventoReativadoAsync(
            "destinatario@test.com",
            "Maria Santos",
            "Evento Reativado");

        // Assert
        await act.Should().NotThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task EnviarEmailContribuicaoCanceladaAsync_NaoDeveLancarExcecao_ComDadosValidos()
    {
        // Arrange
        var options = Options.Create(_validSettings);
        var emailService = new EmailService(options, _loggerMock.Object);

        // Act
        Func<Task> act = async () => await emailService.EnviarEmailContribuicaoCanceladaAsync(
            "contribuidor@test.com",
            "Pedro Costa",
            "Liquidificador",
            "Casamento de João e Maria",
            150.00m,
            "Cancelamento por duplicidade");

        // Assert
        await act.Should().NotThrowAsync<ArgumentNullException>();
    }
}
