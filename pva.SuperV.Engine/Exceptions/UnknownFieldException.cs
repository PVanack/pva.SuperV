using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when an unknown field is referenced.</summary>
    [Serializable]
    public class UnknownFieldException : Exception
    {
        public UnknownFieldException(string? fieldName, string? className) : base($"Field {fieldName} doesn't exist in {className}")
        {
        }

        public UnknownFieldException(string? fieldName) : base($"Field {fieldName} doesn't exist")
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownFieldException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownFieldException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}