using pva.SuperV.Engine;
using pva.SuperV.Model.Instances;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Services.Instances
{
    public class FieldValueService : BaseService, IFieldValueService
    {
        private readonly ILogger logger;

        public FieldValueService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(this.GetType());
        }

        public async Task<FieldModel> GetFieldAsync(string projectId, string instanceName, string fieldName)
        {
            logger.LogDebug("Getting field {FieldName} for {InstanceName} of project {ProjectId}",
                fieldName, instanceName, projectId);
            return await Task.FromResult(FieldMapper.ToDto(GetFieldEntity(projectId, instanceName, fieldName)));
        }

        public async Task<FieldValueModel> UpdateFieldValueAsync(string projectId, string instanceName, string fieldName, FieldValueModel value)
        {
            logger.LogDebug("Updating field value {FieldValue} for {FieldName} for {InstanceName} of project {ProjectId}",
                value.FormattedValue, fieldName, instanceName, projectId);
            IField field = GetFieldEntity(projectId, instanceName, fieldName);
            FieldValueMapper.SetFieldValue(field, value);
            return await Task.FromResult(FieldValueMapper.ToDto(field));
        }

    }
}
