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
        private readonly ILogger logger;

        public FieldProcessingService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(this.GetType());
        }

        public async Task<List<FieldValueProcessingModel>> GetFieldProcessingsAsync(string projectId, string className, string fieldName)
        {
            logger.LogDebug("Getting field processings on field {FieldName} for class {ClassName} of project {ProjectId}",
                fieldName, className, projectId);
            IFieldDefinition fieldDefinition = GetFieldDefinitionEntity(GetProjectEntity(projectId), className, fieldName);
            return await Task.FromResult(fieldDefinition.ValuePostChangeProcessings.ConvertAll(field => FieldProcessingMapper.ToDto(field)));
        }

        public async Task<FieldValueProcessingModel> GetFieldProcessingAsync(string projectId, string className, string fieldName, string processingName)
        {
            logger.LogDebug("Getting field processing {ProcessingName} on field {FieldName} for class {ClassName} of project {ProjectId}",
                processingName, fieldName, className, projectId);
            IFieldDefinition fieldDefinition = GetFieldDefinitionEntity(GetProjectEntity(projectId), className, fieldName);
            IFieldValueProcessing? processing = fieldDefinition.ValuePostChangeProcessings
                .FirstOrDefault(field => field.Name.Equals(processingName));
            return processing is not null
                ? await Task.FromResult(FieldProcessingMapper.ToDto(processing))
                : await Task.FromException<FieldValueProcessingModel>(new UnknownEntityException("FieldValueProcessing", processingName));
        }

        public async Task<FieldValueProcessingModel> CreateFieldProcessingAsync(string projectId, string className, string fieldName, FieldValueProcessingModel createRequest)
        {
            logger.LogDebug("Creating field processing {ProcessingName} on field {FieldName} for class {ClassName} of project {ProjectId}",
                createRequest.Name, fieldName, className, projectId);
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

        public async Task<FieldValueProcessingModel> UpdateFieldProcessingAsync(string projectId, string className, string fieldName, string processingName, FieldValueProcessingModel updateRequest)
        {
            logger.LogDebug("Updating field processing {ProcessingName} on field {FieldName} for class {ClassName} of project {ProjectId}",
                processingName, fieldName, className, projectId);
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                Class clazz = GetClassEntity(wipProject, className);
                IFieldDefinition fieldDefinition = GetFieldDefinitionEntity(clazz, fieldName);
                IFieldValueProcessing fieldProcessing = FieldProcessingMapper.FromDto(wipProject, clazz, fieldDefinition, updateRequest);
                wipProject.UpdateFieldChangePostProcessing(className, fieldName, processingName, fieldProcessing);
                return await Task.FromResult(FieldProcessingMapper.ToDto(fieldProcessing));
            }
            return await Task.FromException<FieldValueProcessingModel>(new NonWipProjectException(projectId));
        }

        public async ValueTask DeleteFieldProcessingAsync(string projectId, string className, string fieldName, string processingName)
        {
            logger.LogDebug("Deleting field processing {ProcessingName} on field {FieldName} for class {ClassName} of project {ProjectId}",
                processingName, fieldName, className, projectId);
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
