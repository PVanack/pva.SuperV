using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    [Serializable]
    public class FieldUsedInOtherProcessingException : SuperVException
    {
        public FieldUsedInOtherProcessingException(string? fieldName, string processingType) : this($"Field {fieldName} is used in another ${processingType} processing")
        {
        }

        public FieldUsedInOtherProcessingException(string message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public FieldUsedInOtherProcessingException()
        {
        }

        [ExcludeFromCodeCoverage]
        public FieldUsedInOtherProcessingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}