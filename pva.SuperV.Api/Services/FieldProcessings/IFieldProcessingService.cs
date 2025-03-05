using pva.SuperV.Model.FieldProcessings;

namespace pva.SuperV.Api.Services.FieldProcessings
{
    public interface IFieldProcessingService
    {
        FieldValueProcessingModel CreateFieldProcessing(string projectId, string className, string fieldName, FieldValueProcessingModel createRequest);
        void DeleteFieldProcessing(string projectId, string className, string fieldName, string processingName);
        FieldValueProcessingModel GetFieldProcessing(string projectId, string className, string fieldName, string processingName);
        List<FieldValueProcessingModel> GetFieldProcessings(string projectId, string className, string fieldName);
    }
}
