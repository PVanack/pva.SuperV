using pva.SuperV.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
