using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when a class with same name already exists in a project.</summary>
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