using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Exceptions
{
    [Serializable]
    public class UnknownFieldException : Exception
    {
        public UnknownFieldException(string? fieldName, string? className) : base($"Field {fieldName} doesn't exist in {className}")
        {
        }

        public UnknownFieldException(string? message) : base(message)
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