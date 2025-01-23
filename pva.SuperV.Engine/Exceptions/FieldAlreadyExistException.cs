using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when a field with same name already exists in a class.</summary>
    [Serializable]
    [SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "Not used anymore")]
    public class FieldAlreadyExistException : Exception
    {
        public FieldAlreadyExistException(string? fieldName) : base($"Field {fieldName} already exists")
        {
        }

        [ExcludeFromCodeCoverage]
        public FieldAlreadyExistException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public FieldAlreadyExistException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}