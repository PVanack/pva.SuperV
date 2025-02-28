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
            Project project = GetProjectEntity(projectId);
            IFieldDefinition fieldDefinition = GetFieldEntity(project, className, fieldName);
            return fieldDefinition.ValuePostChangeProcessings
                .Select(Field => FieldProcessingMapper.ToDto(Field))
                .ToList();
        }

        public FieldValueProcessingModel GetFieldProcessing(string projectId, string className, string fieldName, string processingName)
        {
            Project project = GetProjectEntity(projectId);
            IFieldDefinition fieldDefinition = GetFieldEntity(project, className, fieldName);
            IFieldValueProcessing? processing = fieldDefinition.ValuePostChangeProcessings
                .FirstOrDefault(field => field.Name.Equals(processingName));
            if (processing is not null)
            {
                return FieldProcessingMapper.ToDto(processing);
            }
            else
            {
                throw new UnknownEntityException("FieldValueProcessing", processingName);
            }
        }

        public FieldValueProcessingModel CreateFieldProcessing(string projectId, string className, string fieldName, FieldValueProcessingModel createRequest)
        {
            Project project = GetProjectEntity(projectId);
            if (project is WipProject wipProject)
            {
                Class clazz = GetClassEntity(wipProject, className);
                IFieldValueProcessing fieldProcessing = FieldProcessingMapper.FromDto(project, clazz, createRequest);
                wipProject.AddFieldChangePostProcessing(className, fieldName, fieldProcessing);
                return FieldProcessingMapper.ToDto(fieldProcessing);
            }
            throw new NonWipProjectException(projectId);
        }

        public void DeleteFieldProcessing(string projectId, string className, string fieldName, string processingName)
        {
            Project project = GetProjectEntity(projectId);
            if (project is WipProject wipProject)
            {
                IFieldDefinition fieldDefinition = GetFieldEntity(wipProject, className, fieldName);
                IFieldValueProcessing? processing = fieldDefinition.ValuePostChangeProcessings
                    .FirstOrDefault(field => field.Name.Equals(processingName));
                if (processing is not null)
                {
                    fieldDefinition.ValuePostChangeProcessings.Remove(processing);
                }
                else
                {
                    throw new UnknownEntityException("FieldValueProcessing", processingName);
                }
                return;
            }
            throw new NonWipProjectException(projectId);
        }
    }
}
