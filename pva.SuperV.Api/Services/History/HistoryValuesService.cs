using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.HistoryRetrieval;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Model.HistoryRetrieval;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Services.History
{
    public class HistoryValuesService : BaseService, IHistoryValuesService
    {
        private readonly ILogger logger;

        public HistoryValuesService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(this.GetType());
        }

        public async Task<HistoryRawResultModel> GetInstanceRawHistoryValuesAsync(string projectId, string instanceName, HistoryRequestModel request)
        {
            Project project = GetProjectEntity(projectId);
            if (project is RunnableProject runnableProject)
            {
                Instance instance = runnableProject.GetInstance(instanceName);
                InstanceTimeSerieParameters instanceTimeSerieParameters = RunnableProject.GetHistoryParametersForFields(
                    instance, request.HistoryFields);
                return await GetInstanceRawHistoryValuesAsync(runnableProject, instanceName, request, instanceTimeSerieParameters);
            }
            return await Task.FromException<HistoryRawResultModel>(new NonRunnableProjectException(projectId));
        }

        private async Task<HistoryRawResultModel> GetInstanceRawHistoryValuesAsync(RunnableProject runnableProject, string instanceName, HistoryRequestModel request, InstanceTimeSerieParameters instanceTimeSerieParameters)
        {
            logger.LogDebug("Getting raw history values for instance {InstanceName} of project {ProjectId} between {StartTime} and {EndTime} for fields {FieldNames}",
                instanceName, runnableProject.GetId(), request.StartTime, request.EndTime, String.Join(",", request.HistoryFields));
            HistoryTimeRange query = new(request.StartTime, request.EndTime);
            List<HistoryRow> rows = runnableProject.GetHistoryValues(instanceName, query, instanceTimeSerieParameters);

            int fieldIndex = 0;
            List<HistoryFieldModel> header = [.. instanceTimeSerieParameters.Fields.Select(fieldDefinition => new HistoryFieldModel(fieldDefinition.Name, fieldDefinition.Type.ToString(), fieldIndex++))];
            return await Task.FromResult(new HistoryRawResultModel(header, HistoryRowMapper.ToRawDto(rows)));
        }

        public async Task<HistoryResultModel> GetInstanceHistoryValuesAsync(string projectId, string instanceName, HistoryRequestModel request)
        {
            logger.LogDebug("Getting history values for instance {InstanceName} of project {ProjectId}between {StartTime} and {EndTime} for fields {FieldNames}",
                instanceName, projectId, request.StartTime, request.EndTime, String.Join(",", request.HistoryFields));
            Project project = GetProjectEntity(projectId);
            if (project is RunnableProject runnableProject)
            {
                Instance instance = runnableProject.GetInstance(instanceName);
                InstanceTimeSerieParameters instanceTimeSerieParameters = RunnableProject.GetHistoryParametersForFields(instance, request.HistoryFields);

                HistoryTimeRange query = new(request.StartTime, request.EndTime);
                List<HistoryRow> rows = runnableProject.GetHistoryValues(instanceName, query, instanceTimeSerieParameters);

                int fieldIndex = 0;
                List<HistoryFieldModel> header = [.. instanceTimeSerieParameters.Fields.Select(fieldDefinition => new HistoryFieldModel(fieldDefinition.Name, fieldDefinition.Type.ToString(), fieldIndex++))];
                return await Task.FromResult(new HistoryResultModel(header, HistoryRowMapper.ToDto(rows, instanceTimeSerieParameters.Fields)));
            }
            return await Task.FromException<HistoryResultModel>(new NonRunnableProjectException(projectId));
        }

        public async Task<HistoryStatisticsRawResultModel> GetInstanceRawHistoryStatisticsAsync(string projectId, string instanceName, HistoryStatisticsRequestModel request)
        {
            logger.LogDebug("Getting raw history statistics for instance {InstanceName} of project {ProjectId} between {StartTime} and {EndTime} with an interpolation interval {InterpolationInterval} for fields {FieldNames}",
                instanceName, projectId, request.StartTime, request.EndTime, request.InterpolationInterval, String.Join(",", request.HistoryFields));
            Project project = GetProjectEntity(projectId);
            if (project is RunnableProject runnableProject)
            {
                Instance instance = runnableProject.GetInstance(instanceName);
                InstanceTimeSerieParameters instanceTimeSerieParameters = RunnableProject.GetHistoryParametersForFields(instance, [.. request.HistoryFields.Select(field => field.Name)]);
                HistoryStatisticTimeRange query = new(request.StartTime, request.EndTime, request.InterpolationInterval, request.FillMode);
                int fieldIndex = 0;
                List<HistoryStatisticField> statisticFields = [.. instanceTimeSerieParameters.Fields.Select(fieldDefinition =>
                {
                    int savedFieldIndex = fieldIndex;
                    fieldIndex++;
                    return new HistoryStatisticField(fieldDefinition, request.HistoryFields[savedFieldIndex].StatisticFunction);
                })];

                List<HistoryStatisticRow> rows = runnableProject.GetHistoryStatistics(instanceName, query, statisticFields, instanceTimeSerieParameters);

                return await Task.FromResult(new HistoryStatisticsRawResultModel(BuildStatisticsHeader(request, instanceTimeSerieParameters.Fields, rows), HistoryRowMapper.ToRawDto(rows)));

            }
            return await Task.FromException<HistoryStatisticsRawResultModel>(new NonRunnableProjectException(projectId));
        }

        public async Task<HistoryStatisticsResultModel> GetInstanceHistoryStatisticsAsync(string projectId, string instanceName, HistoryStatisticsRequestModel request)
        {
            logger.LogDebug("Getting history statistics for instance {InstanceName} of project {ProjectId} between {StartTime} and {EndTime} with an interpolation interval {InterpolationInterval} for fields {FieldNames}",
                instanceName, projectId, request.StartTime, request.EndTime, request.InterpolationInterval, String.Join(",", request.HistoryFields));
            Project project = GetProjectEntity(projectId);
            if (project is RunnableProject runnableProject)
            {
                Instance instance = runnableProject.GetInstance(instanceName);
                InstanceTimeSerieParameters instanceTimeSerieParameters = RunnableProject.GetHistoryParametersForFields(instance, [.. request.HistoryFields.Select(field => field.Name)]);
                HistoryStatisticTimeRange query = new(request.StartTime, request.EndTime, request.InterpolationInterval, request.FillMode);
                int fieldIndex = 0;
                List<HistoryStatisticField> statisticFields = [.. instanceTimeSerieParameters.Fields.Select(fieldDefinition =>
                {
                    int savedFieldIndex = fieldIndex;
                    fieldIndex++;
                    return new HistoryStatisticField(fieldDefinition, request.HistoryFields[savedFieldIndex].StatisticFunction);
                })];

                List<HistoryStatisticRow> rows = runnableProject.GetHistoryStatistics(instanceName, query, statisticFields, instanceTimeSerieParameters);

                return await Task.FromResult(new HistoryStatisticsResultModel(BuildStatisticsHeader(request, instanceTimeSerieParameters.Fields, rows),
                    HistoryRowMapper.ToDto(rows, instanceTimeSerieParameters.Fields)));

            }
            return await Task.FromException<HistoryStatisticsResultModel>(new NonRunnableProjectException(projectId));
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
