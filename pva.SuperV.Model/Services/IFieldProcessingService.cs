using pva.SuperV.Model.FieldProcessings;

namespace pva.SuperV.Model.Services
{
    public interface IFieldProcessingService
    {
        Task<FieldValueProcessingModel> CreateFieldProcessingAsync(string projectId, string className, string fieldName, FieldValueProcessingModel createRequest);
        ValueTask DeleteFieldProcessingAsync(string projectId, string className, string fieldName, string processingName);
        Task<FieldValueProcessingModel> GetFieldProcessingAsync(string projectId, string className, string fieldName, string processingName);
        Task<List<FieldValueProcessingModel>> GetFieldProcessingsAsync(string projectId, string className, string fieldName);
        Task<FieldValueProcessingModel> UpdateFieldProcessingAsync(string projectId, string className, string fieldName, string processingName, FieldValueProcessingModel createRequest);
    }
}
