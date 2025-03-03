using pva.SuperV.Model.Instances;

namespace pva.SuperV.Api.Services.Instances
{
    public interface IFieldValueService
    {
        FieldModel GetField(string projectId, string instanceName, string fieldName);
        FieldValueModel UpdateFieldValue(string projectId, string instanceName, string fieldName, FieldValueModel value);
    }
}
