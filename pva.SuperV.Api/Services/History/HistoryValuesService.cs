using pva.SuperV.Engine;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Model.HistoryRetrieval;

namespace pva.SuperV.Api.Services.History
{
    public class HistoryValuesService : BaseService, IHistoryValuesService
    {
        public HistoryRawResultModel GetInstanceRawHistoryValues(string projectId, string instanceName, HistoryRequestModel request)
        {
            Project project = GetProjectEntity(projectId);
            if (project is RunnableProject runnableProject)
            {
                Instance instance = runnableProject.GetInstance(instanceName);
                List<IFieldDefinition> fields;
                HistoryRepository? historyRepository;
                string? classTimeSerieId;
                runnableProject.GetHistoryParametersForFields(instance, request.HistoryFields, out fields, out historyRepository, out classTimeSerieId);
                int fieldIndex = 0;
                List<HistoryFieldModel> header = [.. fields.Select(fieldDefinition =>
                {
                    return new HistoryFieldModel(fieldDefinition.Name, fieldDefinition.Type.ToString(), fieldIndex++);
                })];

                List<HistoryRow> rows = runnableProject.GetHistoryValues(instanceName, request.StartTime, request.EndTime, fields, historyRepository!, classTimeSerieId!);
                return new HistoryRawResultModel(header, HistoryRowMapper.ToRawDto(rows));

            }
            throw new NonRunnableProjectException(projectId);
        }

        public HistoryResultModel GetInstanceHistoryValues(string projectId, string instanceName, HistoryRequestModel request)
        {
            Project project = GetProjectEntity(projectId);
            if (project is RunnableProject runnableProject)
            {
                Instance instance = runnableProject.GetInstance(instanceName);
                List<IFieldDefinition> fields;
                HistoryRepository? historyRepository;
                string? classTimeSerieId;
                runnableProject.GetHistoryParametersForFields(instance, request.HistoryFields, out fields, out historyRepository, out classTimeSerieId);
                int fieldIndex = 0;
                List<HistoryFieldModel> header = [.. fields.Select(fieldDefinition =>
                {
                    return new HistoryFieldModel(fieldDefinition.Name, fieldDefinition.Type.ToString(), fieldIndex++);
                })];

                List<HistoryRow> rows = runnableProject.GetHistoryValues(instanceName, request.StartTime, request.EndTime, fields, historyRepository!, classTimeSerieId!);
                return new HistoryResultModel(header, HistoryRowMapper.ToDto(rows, fields));

            }
            throw new NonRunnableProjectException(projectId);
        }
    }
}
