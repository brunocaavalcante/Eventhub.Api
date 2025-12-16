namespace Eventhub.Domain.Exceptions
{
    public class ExceptionValidation : Exception
    {
        public ExceptionValidation(string message) : base(message) { }
        public ExceptionValidation(string message, Exception innerException) : base(message, innerException) { }
    }
}
