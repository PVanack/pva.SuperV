using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.Projects;

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
            return GetClassEntity(GetProjectEntity(projectId), className);
        }

        protected static Class GetClassEntity(Project project, string className)
        {
            if (project.Classes.TryGetValue(className, out Class? clazz))
            {
                return clazz;
            }
            throw new UnknownEntityException("Class", className);
        }

        protected static IFieldDefinition GetFieldDefinitionEntity(Project project, string className, string fieldName)
        {
            return GetFieldDefinitionEntity(GetClassEntity(project, className), fieldName);
        }

        protected static IFieldDefinition GetFieldDefinitionEntity(Class clazz, string fieldName)
        {
            if (clazz.FieldDefinitions.TryGetValue(fieldName, out IFieldDefinition? fieldDefinition))
            {
                return fieldDefinition;
            }
            throw new UnknownEntityException("Field", fieldName);
        }

        protected static IField GetFieldEntity(string projectId, string instanceName, string fieldName)
        {
            if (GetProjectEntity(projectId) is RunnableProject runnableProject)
            {
                Instance instance = runnableProject.GetInstance(instanceName);
                return instance.GetField(fieldName);
            }
            throw new NonRunnableProjectException(projectId);
        }

        protected static PagedSearchResult<T> CreateResult<T>(ProjectPagedSearchRequest search, List<T> allEntities, List<T> filteredEntities)
            => new(search.PageNumber, search.PageSize, allEntities.Count,
                [.. filteredEntities
                    .Skip((search.PageNumber - 1) * search.PageSize)
                    .Take(search.PageSize)]);

        protected static List<T> SortResult<T>(List<T> entities, string? sortOption, Dictionary<string, Comparison<T>> sortOptions)
        {
            if (String.IsNullOrEmpty(sortOption))
            {
                return entities;
            }
            string actualSortOption = sortOption.StartsWith('-') ? sortOption[1..] : sortOption;
            if (sortOptions.TryGetValue(actualSortOption, out Comparison<T>? comparison))
            {
                entities.Sort(comparison);
                if (sortOption.StartsWith('-'))
                {
                    entities.Reverse();
                }
            }
            else
            {
                throw new InvalidSortOptionException(sortOption, [.. sortOptions!.Keys]);
            }
            return entities;
        }


    }
}
