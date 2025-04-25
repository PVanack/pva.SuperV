using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.Processing;
using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Services.FieldProcessings
{
    public class FieldProcessingService : BaseService, IFieldProcessingService
    {
        public async Task<List<FieldValueProcessingModel>> GetFieldProcessingsAsync(string projectId, string className, string fieldName)
        {
            IFieldDefinition fieldDefinition = GetFieldDefinitionEntity(GetProjectEntity(projectId), className, fieldName);
            return await Task.FromResult(fieldDefinition.ValuePostChangeProcessings.Select(Field => FieldProcessingMapper.ToDto(Field)).ToList());
        }

        public async Task<FieldValueProcessingModel> GetFieldProcessingAsync(string projectId, string className, string fieldName, string processingName)
        {
            IFieldDefinition fieldDefinition = GetFieldDefinitionEntity(GetProjectEntity(projectId), className, fieldName);
            IFieldValueProcessing? processing = fieldDefinition.ValuePostChangeProcessings
                .FirstOrDefault(field => field.Name.Equals(processingName));
            return processing is not null
                ? await Task.FromResult(FieldProcessingMapper.ToDto(processing))
                : await Task.FromException<FieldValueProcessingModel>(new UnknownEntityException("FieldValueProcessing", processingName));
        }

        public async Task<FieldValueProcessingModel> CreateFieldProcessingAsync(string projectId, string className, string fieldName, FieldValueProcessingModel createRequest)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                Class clazz = GetClassEntity(wipProject, className);
                IFieldDefinition fieldDefinition = GetFieldDefinitionEntity(clazz, fieldName);
                IFieldValueProcessing fieldProcessing = FieldProcessingMapper.FromDto(wipProject, clazz, fieldDefinition, createRequest);
                wipProject.AddFieldChangePostProcessing(className, fieldName, fieldProcessing);
                return await Task.FromResult(FieldProcessingMapper.ToDto(fieldProcessing));
            }
            return await Task.FromException<FieldValueProcessingModel>(new NonWipProjectException(projectId));
        }

        public async Task<FieldValueProcessingModel> UpdateFieldProcessingAsync(string projectId, string className, string fieldName, string processingName, FieldValueProcessingModel createRequest)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                Class clazz = GetClassEntity(wipProject, className);
                IFieldDefinition fieldDefinition = GetFieldDefinitionEntity(clazz, fieldName);
                IFieldValueProcessing fieldProcessing = FieldProcessingMapper.FromDto(wipProject, clazz, fieldDefinition, createRequest);
                wipProject.UpdateFieldChangePostProcessing(className, fieldName, processingName, fieldProcessing);
                return await Task.FromResult(FieldProcessingMapper.ToDto(fieldProcessing));
            }
            return await Task.FromException<FieldValueProcessingModel>(new NonWipProjectException(projectId));
        }

        public async ValueTask DeleteFieldProcessingAsync(string projectId, string className, string fieldName, string processingName)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                wipProject.RemoveFieldChangePostProcessing(className, fieldName, processingName);
                await ValueTask.CompletedTask;
                return;
            }
            await ValueTask.FromException(new NonWipProjectException(projectId));
        }
    }
}
