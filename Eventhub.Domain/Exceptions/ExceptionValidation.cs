namespace Eventhub.Domain.Exceptions
{
    public class ExceptionValidation : Exception
    {
        public bool ExibirMsg { get; set; }
        public string MsgErro { get; set; } = string.Empty;

        public ExceptionValidation(string message, bool exibirMsg = false) : base(message) 
        {
            ExibirMsg = exibirMsg;
            MsgErro = message;
        }

        public ExceptionValidation(string message, Exception innerException, bool exibirMsg = false)
            : base(message, innerException)
        {
            ExibirMsg = exibirMsg;
            MsgErro = message;
        }
    }
}
