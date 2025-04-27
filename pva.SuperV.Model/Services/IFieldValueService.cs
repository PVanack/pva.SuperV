using pva.SuperV.Model.Instances;

namespace pva.SuperV.Model.Services
{
    public interface IFieldValueService
    {
        Task<FieldModel> GetFieldAsync(string projectId, string instanceName, string fieldName);
        Task<FieldValueModel> UpdateFieldValueAsync(string projectId, string instanceName, string fieldName, FieldValueModel value);
    }
}
