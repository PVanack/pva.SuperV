using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Exceptions
{
    /// <summary>Exception thrown when name of a project contains invalid character(s).</summary>
    [Serializable]
    public class InvalidProjectNameException : Exception
    {
        public InvalidProjectNameException(string projectName, string projectNamePattern) : base($"Invalid project name {projectName}. Should comply to {projectNamePattern}")
        {
        }

        [ExcludeFromCodeCoverage]
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