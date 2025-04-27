using pva.SuperV.Model.FieldDefinitions;

namespace pva.SuperV.Model.Services
{
    public interface IFieldDefinitionService
    {
        Task<List<FieldDefinitionModel>> CreateFieldsAsync(string projectId, string className, List<FieldDefinitionModel> createRequests);
        ValueTask DeleteFieldAsync(string projectId, string className, string fieldName);
        Task<FieldDefinitionModel> GetFieldAsync(string projectId, string className, string fieldName);
        Task<List<FieldDefinitionModel>> GetFieldsAsync(string projectId, string className);
        Task<PagedSearchResult<FieldDefinitionModel>> SearchFieldsAsync(string projectId, string className, FieldDefinitionPagedSearchRequest search);
        Task<FieldDefinitionModel> UpdateFieldAsync(string projectId, string className, string fieldName, FieldDefinitionModel updateRequest);
    }
}
