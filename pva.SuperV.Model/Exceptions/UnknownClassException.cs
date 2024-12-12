using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Exceptions
{
    [Serializable]
    public class UnknownClassException : Exception
    {
        public UnknownClassException(string? className) : base($"Class {className} doesn't exist")
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownClassException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownClassException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}