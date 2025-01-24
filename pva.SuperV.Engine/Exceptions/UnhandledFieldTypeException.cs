using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when an unknown field is referenced.</summary>
    [Serializable]
    public class UnhandledFieldTypeException : SuperVException
    {
        public UnhandledFieldTypeException(string? fieldName, Type fieldType)
            : base($"Field {fieldName} has unhandled type {fieldType}")
        {
        }

        [ExcludeFromCodeCoverage]
        public UnhandledFieldTypeException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public UnhandledFieldTypeException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public UnhandledFieldTypeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}