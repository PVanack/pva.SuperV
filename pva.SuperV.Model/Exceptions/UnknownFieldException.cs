namespace pva.SuperV.Model.Exceptions
{
    public class UnknownFieldException : Exception
    {
        public UnknownFieldException(string? fieldName, string? className) : base($"Field {fieldName} doesn't exist in {className}")
        {
        }

        public UnknownFieldException() : base()
        {
        }

        public UnknownFieldException(string? message) : base(message)
        {
        }

        public UnknownFieldException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}