using pva.SuperV.Model;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Builder.Exceptions
{
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
