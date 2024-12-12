using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Exceptions
{
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