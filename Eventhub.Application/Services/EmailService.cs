using Eventhub.Application.Helpers;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Eventhub.Application.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
    {
        _settings = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Validar configurações obrigatórias
        if (string.IsNullOrWhiteSpace(_settings.SmtpServer))
            throw new InvalidOperationException("Configuração de Email: SmtpServer não foi definido.");

        if (string.IsNullOrWhiteSpace(_settings.Username))
            throw new InvalidOperationException("Configuração de Email: Username não foi definido.");

        if (string.IsNullOrWhiteSpace(_settings.Password))
            throw new InvalidOperationException("Configuração de Email: Password não foi definido.");

        if (string.IsNullOrWhiteSpace(_settings.FromEmail))
            throw new InvalidOperationException("Configuração de Email: FromEmail não foi definido.");
    }

    public async Task EnviarEmailEventoCanceladoAsync(
        string destinatario, 
        string nomeDestinatario, 
        string nomeEvento, 
        string justificativa)
    {
        // Não enviar emails para endereços temporários (usuários pendentes de cadastro)
        if (EmailHelper.EhEmailTemporario(destinatario))
        {
            _logger.LogDebug(
                "Email de cancelamento não enviado - destinatário com email temporário: {NomeEvento}",
                nomeEvento);
            return;
        }

        try
        {
            var assunto = $"Evento Cancelado: {nomeEvento}";
            var corpoHtml = EmailTemplates.GetEventoCanceladoTemplate(
                nomeDestinatario, 
                nomeEvento, 
                justificativa);

            await EnviarEmailAsync(destinatario, assunto, corpoHtml);
            
            _logger.LogInformation(
                "Email de cancelamento de evento enviado com sucesso para {Destinatario} - Evento: {NomeEvento}", 
                destinatario, 
                nomeEvento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Erro ao enviar email de cancelamento de evento para {Destinatario} - Evento: {NomeEvento}", 
                destinatario, 
                nomeEvento);
            // Não relançar a exceção para não bloquear o fluxo principal
        }
    }

    public async Task EnviarEmailEventoReativadoAsync(
        string destinatario, 
        string nomeDestinatario, 
        string nomeEvento)
    {
        // Não enviar emails para endereços temporários (usuários pendentes de cadastro)
        if (EmailHelper.EhEmailTemporario(destinatario))
        {
            _logger.LogDebug(
                "Email de reativação não enviado - destinatário com email temporário: {NomeEvento}",
                nomeEvento);
            return;
        }

        try
        {
            var assunto = $"Evento Reativado: {nomeEvento}";
            var corpoHtml = EmailTemplates.GetEventoReativadoTemplate(
                nomeDestinatario, 
                nomeEvento);

            await EnviarEmailAsync(destinatario, assunto, corpoHtml);
            
            _logger.LogInformation(
                "Email de reativação de evento enviado com sucesso para {Destinatario} - Evento: {NomeEvento}", 
                destinatario, 
                nomeEvento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Erro ao enviar email de reativação de evento para {Destinatario} - Evento: {NomeEvento}", 
                destinatario, 
                nomeEvento);
            // Não relançar a exceção para não bloquear o fluxo principal
        }
    }

    public async Task EnviarEmailEventoExcluidoAsync(
        string destinatario, 
        string nomeDestinatario, 
        string nomeEvento, 
        DateTime dataEvento)
    {
        // Não enviar emails para endereços temporários (usuários pendentes de cadastro)
        if (EmailHelper.EhEmailTemporario(destinatario))
        {
            _logger.LogDebug(
                "Email de exclusão não enviado - destinatário com email temporário: {NomeEvento}",
                nomeEvento);
            return;
        }

        try
        {
            var assunto = $"Evento Excluído: {nomeEvento}";
            var corpoHtml = EmailTemplates.GetEventoExcluidoTemplate(
                nomeDestinatario, 
                nomeEvento, 
                dataEvento);

            await EnviarEmailAsync(destinatario, assunto, corpoHtml);
            
            _logger.LogInformation(
                "Email de exclusão de evento enviado com sucesso para {Destinatario} - Evento: {NomeEvento}", 
                destinatario, 
                nomeEvento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Erro ao enviar email de exclusão de evento para {Destinatario} - Evento: {NomeEvento}", 
                destinatario, 
                nomeEvento);
        }
    }

    public async Task EnviarEmailContribuicaoCanceladaAsync(
        string destinatario, 
        string nomeDestinatario, 
        string nomePresente, 
        string nomeEvento, 
        decimal valorContribuicao, 
        string justificativa)
    {
        // Não enviar emails para endereços temporários (usuários pendentes de cadastro)
        if (EmailHelper.EhEmailTemporario(destinatario))
        {
            _logger.LogDebug(
                "Email de cancelamento de contribuição não enviado - destinatário com email temporário: {NomePresente}",
                nomePresente);
            return;
        }

        try
        {
            var assunto = $"Contribuição Cancelada: {nomePresente}";
            var corpoHtml = EmailTemplates.GetContribuicaoCanceladaTemplate(
                nomeDestinatario, 
                nomePresente, 
                nomeEvento, 
                valorContribuicao, 
                justificativa);

            await EnviarEmailAsync(destinatario, assunto, corpoHtml);
            
            _logger.LogInformation(
                "Email de cancelamento de contribuição enviado com sucesso para {Destinatario} - Presente: {NomePresente}, Valor: {Valor}", 
                destinatario, 
                nomePresente, 
                valorContribuicao);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Erro ao enviar email de cancelamento de contribuição para {Destinatario} - Presente: {NomePresente}", 
                destinatario, 
                nomePresente);
            // Não relançar a exceção para não bloquear o fluxo principal
        }
    }

    private async Task EnviarEmailAsync(string destinatario, string assunto, string corpoHtml)
    {
        using var smtpClient = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
        {
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = _settings.EnableSsl
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_settings.FromEmail, _settings.FromName),
            Subject = assunto,
            Body = corpoHtml,
            IsBodyHtml = true
        };

        mailMessage.To.Add(destinatario);

        await smtpClient.SendMailAsync(mailMessage);
    }
}
