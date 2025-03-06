using pva.SuperV.Engine.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Api
{
    /// <summary>Exception thrown when trying to build from a non runnable project.</summary>
    public class NonRunnableProjectException : SuperVException
    {
        public NonRunnableProjectException(string projectId)
            : base($"Project with ID {projectId} is not a runnable project")
        {
        }

        [ExcludeFromCodeCoverage]
        public NonRunnableProjectException()
        {
        }

        [ExcludeFromCodeCoverage]
        public NonRunnableProjectException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}