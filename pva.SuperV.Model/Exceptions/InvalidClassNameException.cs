namespace pva.SuperV.Model.Exceptions
{
    [Serializable]
    public class InvalidClassNameException : Exception
    {
        public InvalidClassNameException()
        {
        }

        public InvalidClassNameException(string? message) : base(message)
        {
        }

        public InvalidClassNameException(string className, string classNamePattern) : base($"Invalid class name {className}. Should comply to {classNamePattern}")
        {
        }

        public InvalidClassNameException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}