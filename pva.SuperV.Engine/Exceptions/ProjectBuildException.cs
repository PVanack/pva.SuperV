using pva.SuperV.Engine;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when an error occurs while building a project.</summary>
    [Serializable]
    [ExcludeFromCodeCoverage(Justification = "Impossible to create a Roslyn compilation error by unit test.")]
    public class ProjectBuildException : Exception
    {
        public ProjectBuildException(Project project, string diagnostics) : base($"Error building project {project.Name} : {diagnostics}")
        {
        }

        public ProjectBuildException(string? message) : base(message)
        {
        }

        public ProjectBuildException() : base()
        {
        }

        public ProjectBuildException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}