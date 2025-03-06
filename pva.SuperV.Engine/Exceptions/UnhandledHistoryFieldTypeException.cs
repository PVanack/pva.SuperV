using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when an unhandled field type for history storage engine is referenced.</summary>
    public class UnhandledHistoryFieldTypeException : SuperVException
    {
        public UnhandledHistoryFieldTypeException(string? fieldName, Type fieldType)
            : base($"Field {fieldName} has unhandled type {fieldType} by history storage engine.")
        {
        }

        [ExcludeFromCodeCoverage]
        public UnhandledHistoryFieldTypeException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public UnhandledHistoryFieldTypeException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public UnhandledHistoryFieldTypeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}