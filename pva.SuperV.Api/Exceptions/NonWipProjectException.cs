using pva.SuperV.Engine.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Api.Exceptions
{
    /// <summary>Exception thrown when trying to build from a non WIP project.</summary>
    public class NonWipProjectException : SuperVException
    {
        public NonWipProjectException(string projectId)
            : base($"Project with ID {projectId} is not a WIP project")
        {
        }

        [ExcludeFromCodeCoverage]
        public NonWipProjectException()
        {
        }

        [ExcludeFromCodeCoverage]
        public NonWipProjectException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}