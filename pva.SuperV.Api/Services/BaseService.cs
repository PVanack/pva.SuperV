using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Api.Services
{
    public abstract class BaseService
    {
        protected static Project GetProjectEntity(string projectId)
        {
            if (Project.Projects.TryGetValue(projectId, out Project? project))
            {
                return project;
            }
            throw new UnknownEntityException("Project", projectId);
        }

        protected static Class GetClassEntity(string projectId, string className)
        {
            Project project = GetProjectEntity(projectId);
            return GetClassEntity(project, className);
        }

        protected static Class GetClassEntity(Project project, string className)
        {
            if (project.Classes.TryGetValue(className, out Class? clazz))
            {
                return clazz;
            }
            throw new UnknownEntityException("Class", className);
        }

        protected static IFieldDefinition GetFieldEntity(Project project, string className, string fieldName)
        {
            Class clazz = GetClassEntity(project, className);
            if (clazz.FieldDefinitions.TryGetValue(fieldName, out IFieldDefinition? fieldDefinition))
            {
                return fieldDefinition;
            }
            throw new UnknownEntityException("Field", fieldName);
        }
    }
}
