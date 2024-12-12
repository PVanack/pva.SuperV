using pva.SuperV.Model;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Builder.Exceptions
{
    [Serializable]
    public class ProjectBuildException : Exception
    {
        public ProjectBuildException(Project project, String diagnostics) : this($"Error building project {project.Name} : {diagnostics}")
        {
        }

        public ProjectBuildException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public ProjectBuildException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public ProjectBuildException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
