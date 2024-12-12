using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Exceptions
{
    [Serializable]
    public class InvalidClassNameException : Exception
    {
        public InvalidClassNameException(string className, string classNamePattern) : this($"Invalid class name {className}. Should comply to {classNamePattern}")
        {
        }

        public InvalidClassNameException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidClassNameException()
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidClassNameException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}