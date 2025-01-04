using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when name of a field formatter contains invalid character(s).</summary>
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