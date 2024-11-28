namespace pva.SuperVAccess.Exceptions
{
    public class ClassAlreadyExistException : Exception
    {
        public ClassAlreadyExistException(string? className) : base($"Class {className} already exists")
        {
        }

        public ClassAlreadyExistException() : base()
        {
        }

        public ClassAlreadyExistException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}