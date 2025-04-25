using pva.SuperV.Engine;
using pva.SuperV.Model.Instances;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Services.Instances
{
    public class FieldValueService : BaseService, IFieldValueService
    {
        public async Task<FieldModel> GetFieldAsync(string projectId, string instanceName, string fieldName)
        {
            return await Task.FromResult(FieldMapper.ToDto(GetFieldEntity(projectId, instanceName, fieldName)));
        }

        public async Task<FieldValueModel> UpdateFieldValueAsync(string projectId, string instanceName, string fieldName, FieldValueModel value)
        {
            IField field = GetFieldEntity(projectId, instanceName, fieldName);
            FieldValueMapper.SetFieldValue(field, value);
            return await Task.FromResult(FieldValueMapper.ToDto(field));
        }

    }
}
