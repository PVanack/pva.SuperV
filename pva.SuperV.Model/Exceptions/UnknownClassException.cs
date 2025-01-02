using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Exceptions
{
    /// <summary>Exception thrown when an unknown class is referenced.</summary>
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