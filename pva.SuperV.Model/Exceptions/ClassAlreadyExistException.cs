using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Exceptions
{
    [Serializable]
    public class ClassAlreadyExistException : Exception
    {
        public ClassAlreadyExistException(string? className) : base($"Class {className} already exists")
        {
        }

        [ExcludeFromCodeCoverage]
        public ClassAlreadyExistException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public ClassAlreadyExistException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}