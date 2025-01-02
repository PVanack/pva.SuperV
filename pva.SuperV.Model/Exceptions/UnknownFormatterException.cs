using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Exceptions
{
    /// <summary>Exception thrown when an unknown field formatter is referenced.</summary>
    [Serializable]
    public class UnknownFormatterException : Exception
    {
        public UnknownFormatterException(string? fieldFormatterName) : base($"Field formatter {fieldFormatterName} doesn't exist")
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownFormatterException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownFormatterException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}