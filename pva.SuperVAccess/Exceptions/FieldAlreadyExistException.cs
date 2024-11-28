namespace pva.SuperVAccess.Exceptions
{
    public class FieldAlreadyExistException : Exception
    {
        public FieldAlreadyExistException(string? fieldName) : base($"Field {fieldName} already exists")
        {
        }

        public FieldAlreadyExistException() : base()
        {
        }

        public FieldAlreadyExistException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}