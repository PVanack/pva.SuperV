using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldDefinitions;

namespace pva.SuperV.Api.Services.FieldDefinitions
{
    public class FieldDefinitionService : BaseService, IFieldDefinitionService
    {
        public List<FieldDefinitionModel> GetFields(string projectId, string className)
        {
            Class clazz = GetClassEntity(projectId, className);
            return [.. clazz.FieldDefinitions.Values.Select(field => FieldDefinitionMapper.ToDto(field!))];
        }

        public FieldDefinitionModel GetField(string projectId, string className, string fieldName)
        {
            Class clazz = GetClassEntity(projectId, className);
            if (clazz.FieldDefinitions.TryGetValue(fieldName, out IFieldDefinition? fieldDefinition))
            {
                return FieldDefinitionMapper.ToDto(fieldDefinition);
            }
            throw new UnknownEntityException("Field", fieldName);
        }

        public List<FieldDefinitionModel> CreateFields(string projectId, string className, List<FieldDefinitionModel> createRequests)
        {
            Project project = GetProjectEntity(projectId);
            if (project is WipProject wipProject)
            {
                List<FieldDefinitionModel> createdFieldDefinitions = [];
                Class clazz = GetClassEntity(wipProject, className);
                try
                {
                    createRequests.ForEach(fieldDefinition =>
                    {
                        FieldFormatter? fieldFormatter = null;
                        if (fieldDefinition.ValueFormatter is not null)
                        {
                            fieldFormatter = wipProject.GetFormatter(fieldDefinition.ValueFormatter);
                        }
                        createdFieldDefinitions.Add(FieldDefinitionMapper.ToDto(clazz.AddField(FieldDefinitionMapper.FromDto(fieldDefinition), fieldFormatter)));
                    });
                    return createdFieldDefinitions;
                }
                catch (SuperVException)
                {
                    // If exception while creatig one of the fields, remove all the already created fields.
                    try
                    {
                        createdFieldDefinitions.ForEach(fieldDefinition =>
                        {
                            clazz.RemoveField(fieldDefinition.Name);
                        });
                    }
                    catch (SuperVException)
                    {
                        // Ignore execption while deleting
                    }
                    throw;
                }
            }
            throw new NonWipProjectException(projectId);
        }

        public void DeleteField(string projectId, string className, string fieldName)
        {
            Project project = GetProjectEntity(projectId);
            if (project is WipProject wipProject)
            {
                Class clazz = GetClassEntity(wipProject, className);
                clazz.RemoveField(fieldName);
                return;
            }
            throw new NonWipProjectException(projectId);
        }
    }
}
