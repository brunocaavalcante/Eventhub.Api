namespace Eventhub.Domain.Exceptions
{
    public class ExceptionValidation : Exception
    {
        public bool ExibirMsg { get; set; }
        public ExceptionValidation(string message, bool exibirMsg = false) : base(message) 
        {
            ExibirMsg = exibirMsg;
        }
        public ExceptionValidation(string message, Exception innerException) : base(message, innerException) { }
    }
}
