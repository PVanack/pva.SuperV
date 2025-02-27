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
            return clazz.FieldDefinitions.Values
                .Select(field => FieldDefinitionMapper.ToDto(field!))
                .ToList();
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

        public FieldDefinitionModel CreateField(string projectId, string className, FieldDefinitionModel createRequest)
        {
            Project project = GetProjectEntity(projectId);
            if (project is WipProject wipProject)
            {
                Class clazz = GetClassEntity(wipProject, className);
                IFieldDefinition fieldDefinition = clazz.AddField(FieldDefinitionMapper.FromDto(createRequest));
                return FieldDefinitionMapper.ToDto(fieldDefinition);

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
