using pva.SuperV.Model;
using pva.SuperV.Model.FieldDefinitions;

namespace pva.SuperV.Api.Services.FieldDefinitions
{
    public interface IFieldDefinitionService
    {
        List<FieldDefinitionModel> CreateFields(string projectId, string className, List<FieldDefinitionModel> createRequests);
        void DeleteField(string projectId, string className, string fieldName);
        FieldDefinitionModel GetField(string projectId, string className, string fieldName);
        List<FieldDefinitionModel> GetFields(string projectId, string className);
        PagedSearchResult<FieldDefinitionModel> SearchFields(string projectId, string className, FieldDefinitionPagedSearchRequest search);
        FieldDefinitionModel UpdateField(string projectId, string className, string fieldName, FieldDefinitionModel updateRequest);
    }
}
