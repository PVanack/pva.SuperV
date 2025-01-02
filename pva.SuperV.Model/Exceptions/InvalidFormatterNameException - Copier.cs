using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Exceptions
{
    [Serializable]
    public class InvalidTypeForFormatterException : Exception
    {
        public InvalidTypeForFormatterException(Type fieldType, string fieldFormatterName) :
            base($"Invalid type {fieldType.Name} for formatter {fieldFormatterName}.")
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