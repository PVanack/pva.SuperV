using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when type requested doesn't match actual field type.</summary>
    [Serializable]
    public class StringConversionException : SuperVException
    {
        public StringConversionException(string fieldName, string? stringValue, Type fieldType)
            : base($"Can't convert {stringValue} for {fieldName} with type {fieldType}")
        {
        }

        [ExcludeFromCodeCoverage]
        public StringConversionException()
        {
        }

        [ExcludeFromCodeCoverage]
        public StringConversionException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public StringConversionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}