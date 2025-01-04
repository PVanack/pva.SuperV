using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when a field with same name already exists in a class.</summary>
    [Serializable]
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