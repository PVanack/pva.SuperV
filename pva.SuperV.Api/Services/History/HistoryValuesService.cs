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
                int fieldIndex = 0;
                List<HistoryFieldModel> header = [.. request.HistoryFields.Select(fieldName =>
                {
                    IField field = instance.GetField(fieldName);
                    return new HistoryFieldModel(fieldName, field.Type.ToString(), fieldIndex++);
                })];
                List<HistoryRow> rows = runnableProject.GetHistoryValues(instanceName, request.StartTime, request.EndTime,
                        request.HistoryFields);
                return new HistoryRawResultModel(header, HistoryRawRowMapper.ToDto(rows));

            }
            throw new NonRunnableProjectException(projectId);
        }
    }
}
