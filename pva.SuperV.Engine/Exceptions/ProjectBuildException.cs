using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when an error occurs while building a project.</summary>
    [ExcludeFromCodeCoverage]
    public class ProjectBuildException : SuperVException
    {
        public ProjectBuildException(Project project, string diagnostics)
            : base($"Error building project {project.Name} : {diagnostics}")
        {
        }

        public ProjectBuildException(string? message) : base(message)
        {
        }

        public ProjectBuildException()
        {
        }

        public ProjectBuildException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}