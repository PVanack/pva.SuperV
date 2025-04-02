using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.Processing;
using pva.SuperV.Model.FieldProcessings;

namespace pva.SuperV.Api.Services.FieldProcessings
{
    public class FieldProcessingService : BaseService, IFieldProcessingService
    {
        public List<FieldValueProcessingModel> GetFieldProcessings(string projectId, string className, string fieldName)
        {
            IFieldDefinition fieldDefinition = GetFieldDefinitionEntity(GetProjectEntity(projectId), className, fieldName);
            return [.. fieldDefinition.ValuePostChangeProcessings.Select(Field => FieldProcessingMapper.ToDto(Field))];
        }

        public FieldValueProcessingModel GetFieldProcessing(string projectId, string className, string fieldName, string processingName)
        {
            IFieldDefinition fieldDefinition = GetFieldDefinitionEntity(GetProjectEntity(projectId), className, fieldName);
            IFieldValueProcessing? processing = fieldDefinition.ValuePostChangeProcessings
                .FirstOrDefault(field => field.Name.Equals(processingName));
            return processing is not null
                ? FieldProcessingMapper.ToDto(processing)
                : throw new UnknownEntityException("FieldValueProcessing", processingName);
        }

        public FieldValueProcessingModel CreateFieldProcessing(string projectId, string className, string fieldName, FieldValueProcessingModel createRequest)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                Class clazz = GetClassEntity(wipProject, className);
                IFieldDefinition fieldDefinition = GetFieldDefinitionEntity(clazz, fieldName);
                IFieldValueProcessing fieldProcessing = FieldProcessingMapper.FromDto(wipProject, clazz, fieldDefinition, createRequest);
                wipProject.AddFieldChangePostProcessing(className, fieldName, fieldProcessing);
                return FieldProcessingMapper.ToDto(fieldProcessing);
            }
            throw new NonWipProjectException(projectId);
        }

        public void DeleteFieldProcessing(string projectId, string className, string fieldName, string processingName)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                wipProject.RemoveFieldChangePostProcessing(className, fieldName, processingName);
                return;
            }
            throw new NonWipProjectException(projectId);
        }
    }
}
