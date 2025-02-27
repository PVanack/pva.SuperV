using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Api.Services
{
    public abstract class BaseService
    {
        protected Project GetProjectEntity(string projectId)
        {
            if (Project.Projects.TryGetValue(projectId, out Project? project))
            {
                return project;
            }
            throw new UnknownEntityException("Project", projectId);
        }

        protected Class GetClassEntity(string projectId, string className)
        {
            Project project = GetProjectEntity(projectId);
            return GetClassEntity(project, className);
        }

        protected Class GetClassEntity(Project project, string className)
        {
            if (project.Classes.TryGetValue(className, out Class? clazz))
            {
                return clazz;
            }
            throw new UnknownEntityException("Class", className);
        }
    }
}
