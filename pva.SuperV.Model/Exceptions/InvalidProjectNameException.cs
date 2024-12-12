using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Exceptions
{
    [Serializable]
    public class InvalidProjectNameException : Exception
    {
        public InvalidProjectNameException(string projectName, string projectNamePattern) : this($"Invalid project name {projectName}. Should comply to {projectNamePattern}")
        {
        }

        public InvalidProjectNameException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidProjectNameException()
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidProjectNameException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}