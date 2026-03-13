namespace Eventhub.Application.Interfaces;

public interface IEmailService
{
    Task EnviarEmailEventoCanceladoAsync(
        string destinatario, 
        string nomeDestinatario, 
        string nomeEvento, 
        string justificativa);

    Task EnviarEmailEventoReativadoAsync(
        string destinatario, 
        string nomeDestinatario, 
        string nomeEvento);

    Task EnviarEmailContribuicaoCanceladaAsync(
        string destinatario, 
        string nomeDestinatario, 
        string nomePresente, 
        string nomeEvento, 
        decimal valorContribuicao, 
        string justificativa);
}
