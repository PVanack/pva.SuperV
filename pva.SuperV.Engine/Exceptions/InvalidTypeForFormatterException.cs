using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when field on which a formatter is applied is not in the allowed types of formatter.</summary>
    [Serializable]
    public class InvalidTypeForFormatterException : SuperVException
    {
        public InvalidTypeForFormatterException(Type fieldType, string fieldFormatterName)
            : base($"Invalid type {fieldType.Name} for formatter {fieldFormatterName}.")
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidTypeForFormatterException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidTypeForFormatterException()
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidTypeForFormatterException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}