namespace pva.SuperV.Model.Exceptions
{
    public class UnknownClassException: Exception
    {
        public UnknownClassException(string? className) : base($"Class {className} doesn't exist")
        {
        }

        public UnknownClassException() : base()
        {
        }

        public UnknownClassException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}