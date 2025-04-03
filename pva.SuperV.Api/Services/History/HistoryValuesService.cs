using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.HistoryRetrieval;
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
                RunnableProject.GetHistoryParametersForFields(instance, request.HistoryFields,
                    out List<IFieldDefinition> fields, out HistoryRepository? historyRepository, out string? classTimeSerieId);
                HistoryTimeRange query = new(request.StartTime, request.EndTime);
                List<HistoryRow> rows = runnableProject.GetHistoryValues(instanceName, query, fields, historyRepository!, classTimeSerieId!);

                int fieldIndex = 0;
                List<HistoryFieldModel> header = [.. fields.Select(fieldDefinition =>
                {
                    return new HistoryFieldModel(fieldDefinition.Name, fieldDefinition.Type.ToString(), fieldIndex++);
                })];
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
                RunnableProject.GetHistoryParametersForFields(instance, request.HistoryFields,
                    out List<IFieldDefinition> fields, out HistoryRepository? historyRepository, out string? classTimeSerieId);

                HistoryTimeRange query = new(request.StartTime, request.EndTime);
                List<HistoryRow> rows = runnableProject.GetHistoryValues(instanceName, query, fields, historyRepository!, classTimeSerieId!);

                int fieldIndex = 0;
                List<HistoryFieldModel> header = [.. fields.Select(fieldDefinition =>
                {
                    return new HistoryFieldModel(fieldDefinition.Name, fieldDefinition.Type.ToString(), fieldIndex++);
                })];
                return new HistoryResultModel(header, HistoryRowMapper.ToDto(rows, fields));

            }
            throw new NonRunnableProjectException(projectId);
        }

        public HistoryStatisticsRawResultModel GetInstanceRawHistoryStatistics(string projectId, string instanceName, HistoryStatisticsRequestModel request)
        {
            Project project = GetProjectEntity(projectId);
            if (project is RunnableProject runnableProject)
            {
                Instance instance = runnableProject.GetInstance(instanceName);
                RunnableProject.GetHistoryParametersForFields(instance, [.. request.HistoryFields.Select(field => field.Name)],
                    out List<IFieldDefinition> fields, out HistoryRepository? historyRepository, out string? classTimeSerieId);
                HistoryStatisticTimeRange query = new(request.StartTime, request.EndTime, request.InterpolationInterval, request.FillMode);
                int fieldIndex = 0;
                List<HistoryStatisticField> statisticFields = [.. fields.Select(fieldDefinition =>
                {
                    int savedFieldIndex = fieldIndex;
                    fieldIndex++;
                    return new HistoryStatisticField(fieldDefinition, request.HistoryFields[savedFieldIndex].StatisticFunction);
                })];

                List<HistoryStatisticRow> rows = runnableProject.GetHistoryStatistics(instanceName, query, statisticFields, historyRepository!, classTimeSerieId!);

                return new HistoryStatisticsRawResultModel(BuildStatisticsHeader(request, fields, rows), HistoryRowMapper.ToRawDto(rows));

            }
            throw new NonRunnableProjectException(projectId);
        }

        public HistoryStatisticsResultModel GetInstanceHistoryStatistics(string projectId, string instanceName, HistoryStatisticsRequestModel request)
        {
            Project project = GetProjectEntity(projectId);
            if (project is RunnableProject runnableProject)
            {
                Instance instance = runnableProject.GetInstance(instanceName);
                RunnableProject.GetHistoryParametersForFields(instance, [.. request.HistoryFields.Select(field => field.Name)],
                    out List<IFieldDefinition> fields, out HistoryRepository? historyRepository, out string? classTimeSerieId);
                HistoryStatisticTimeRange query = new(request.StartTime, request.EndTime, request.InterpolationInterval, request.FillMode);
                int fieldIndex = 0;
                List<HistoryStatisticField> statisticFields = [.. fields.Select(fieldDefinition =>
                {
                    int savedFieldIndex = fieldIndex;
                    fieldIndex++;
                    return new HistoryStatisticField(fieldDefinition, request.HistoryFields[savedFieldIndex].StatisticFunction);
                })];

                List<HistoryStatisticRow> rows = runnableProject.GetHistoryStatistics(instanceName, query, statisticFields, historyRepository!, classTimeSerieId!);

                return new HistoryStatisticsResultModel(BuildStatisticsHeader(request, fields, rows), HistoryRowMapper.ToDto(rows, fields));

            }
            throw new NonRunnableProjectException(projectId);
        }

        private static List<HistoryStatisticResultFieldModel> BuildStatisticsHeader(HistoryStatisticsRequestModel request, List<IFieldDefinition> fields, List<HistoryStatisticRow> rows)
        {
            HistoryStatisticRow? firstRow = rows.FirstOrDefault();
            ArgumentNullException.ThrowIfNull(firstRow);
            int fieldIndex = 0;
            return [.. fields.Select(fieldDefinition =>
                {
                    object? valueAsObject = firstRow.Values[fieldIndex];
                    HistoryStatisticResultFieldModel historyStatisticResultFieldModel = new(fieldDefinition.Name,
                        firstRow is not null && valueAsObject is not null
                        ? valueAsObject.GetType().ToString()
                        : fieldDefinition.Type.ToString(), fieldIndex, request.HistoryFields[fieldIndex].StatisticFunction);
                    fieldIndex++;
                    return historyStatisticResultFieldModel;
                })];
        }
    }
}
