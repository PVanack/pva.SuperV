using pva.SuperV.Model;

namespace pva.SuperV.Builder.Exceptions
{
    public class ProjectBuildException : Exception
    {
        public ProjectBuildException(Project project, String diagnostics) : base($"Error building project {project.Name} : {diagnostics}")
        {
        }

        public ProjectBuildException() : base()
        {
        }

        public ProjectBuildException(string? message) : base(message)
        {
        }

        public ProjectBuildException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
