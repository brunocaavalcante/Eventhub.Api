using Eventhub.Domain.Exceptions;

namespace Eventhub.Api.Models;

public class ExceptionValidationDto
{
    public bool ExibirMsg { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public string MsgErro { get; set; } = string.Empty;

    public static ExceptionValidationDto FromException(ExceptionValidation ex)
    {
        return new ExceptionValidationDto
        {
            ExibirMsg = ex.ExibirMsg,
            Mensagem = ex.Message,
            MsgErro = string.IsNullOrWhiteSpace(ex.MsgErro) ? ex.Message : ex.MsgErro
        };
    }

    public static ExceptionValidationDto FromMessage(string mensagem, bool exibirMsg = true)
    {
        return new ExceptionValidationDto
        {
            ExibirMsg = exibirMsg,
            Mensagem = mensagem,
            MsgErro = mensagem
        };
    }
}
