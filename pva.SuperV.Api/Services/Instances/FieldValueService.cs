using pva.SuperV.Engine;
using pva.SuperV.Model.Instances;

namespace pva.SuperV.Api.Services.Instances
{
    public class FieldValueService : BaseService, IFieldValueService
    {
        public FieldValueModel GetField(string projectId, string instanceName, string fieldName)
        {
            return FieldValueMapper.ToDto(GetFieldEntity(projectId, instanceName, fieldName));
        }

        public FieldValueModel UpdateFieldValue(string projectId, string instanceName, string fieldName, FieldValueModel value)
        {
            IField field = GetFieldEntity(projectId, instanceName, fieldName);
            FieldValueMapper.SetFieldValue(field, value);
            return FieldValueMapper.ToDto(field);
        }

    }
}
