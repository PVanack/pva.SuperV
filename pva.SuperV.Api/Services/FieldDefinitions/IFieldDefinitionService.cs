using pva.SuperV.Model.FieldDefinitions;

namespace pva.SuperV.Api.Services.FieldDefinitions
{
    public interface IFieldDefinitionService
    {
        FieldDefinitionModel CreateField(string projectId, string className, FieldDefinitionModel createRequest);
        void DeleteField(string projectId, string className, string fieldName);
        FieldDefinitionModel GetField(string projectId, string className, string fieldName);
        List<FieldDefinitionModel> GetFields(string projectId, string className);
    }
}
