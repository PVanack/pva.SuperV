using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when an unhandled field type for history storage engine is referenced.</summary>
    public class UnhandledFieldTypeException : SuperVException
    {
        public UnhandledFieldTypeException(string? fieldName, Type? fieldType)
            : base($"Field {fieldName} type {fieldType} is not handled by SuperV engine.")
        {
        }

        [ExcludeFromCodeCoverage]
        public UnhandledFieldTypeException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public UnhandledFieldTypeException()
        {
        }

        [ExcludeFromCodeCoverage]
        public UnhandledFieldTypeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}