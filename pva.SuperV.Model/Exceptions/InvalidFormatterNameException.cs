using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Exceptions
{
    [Serializable]
    public class InvalidFormatterNameException : Exception
    {
        public InvalidFormatterNameException(string fieldFormatterName, string fieldFormatterNamePattern) :
            base($"Invalid field formatter name {fieldFormatterName}. Should comply to {fieldFormatterNamePattern}")
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidFormatterNameException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidFormatterNameException()
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidFormatterNameException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}