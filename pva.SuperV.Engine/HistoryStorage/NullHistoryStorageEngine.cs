using pva.SuperV.Engine.HistoryRetrieval;
using pva.SuperV.Engine.Processing;

namespace pva.SuperV.Engine.HistoryStorage
{
    /// <summary>
    /// Null history storage engine doing nothing. Used for unit tests.
    /// </summary>
    public class NullHistoryStorageEngine : IHistoryStorageEngine
    {
        /// <summary>
        /// Null history storage string.
        /// </summary>
        public const string Prefix = "NullHistoryStorage";

        /// <summary>
        /// Upsert a history repository in storage engine.
        /// </summary>
        /// <param name="projectName">Project name to which the repository belongs.</param>
        /// <param name="repository">History repository</param>
        /// <returns>ID of repository in storage engine.</returns>
        public string UpsertRepository(string projectName, HistoryRepository repository)
        {
            return $"{projectName}_{repository.Name}";
        }

        /// <summary>
        /// Deletes a history repository from storage engine.
        /// </summary>
        /// <param name="projectName">Project name to zhich the repository belongs.</param>
        /// <param name="repositoryName">History repository name.</param>
        public void DeleteRepository(string projectName, string repositoryName)
        {
        }

        /// <summary>
        /// Upsert a class time series in storage engine
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repositoryStorageId">History respository in which the time series should be created.</param>
        /// <param name="projectName">Project name to zhich the time series belongs.</param>
        /// <param name="className">Class name</param>
        /// <param name="historizationProcessing">History processing for which the time series should be created.</param>
        /// <returns>Time series ID in storage engine.</returns>
        public string UpsertClassTimeSerie<T>(string repositoryStorageId, string projectName, string className, HistorizationProcessing<T> historizationProcessing)
        {
            return $"{projectName}_{className}";
        }

        /// <summary>
        /// Historize instance values in storage engine
        /// </summary>
        /// <param name="repositoryStorageId">The history repository ID.</param>
        /// <param name="historizationProcessingName">The historization processing name.</param>
        /// <param name="classTimeSerieId">The time series ID.</param>
        /// <param name="instanceName">The instance name.</param>
        /// <param name="timestamp">the timestamp of the values</param>
        /// <param name="quality">The quality of the values.</param>
        /// <param name="fieldsToHistorize">List of fields to be historized.</param>
        public void HistorizeValues(string repositoryStorageId, string historizationProcessingName, string classTimeSerieId, string instanceName, DateTime timestamp, QualityLevel? quality, List<IField> fieldsToHistorize)
        {
        }

        /// <summary>
        /// Gets instance values historized between 2 timestamps.
        /// </summary>
        /// <param name="instanceName">The instance name.</param>
        /// <param name="timeRange">Query containing time range parameters.</param>
        /// <param name="instanceTimeSerieParameters">Parameters defining the time serie.</param>
        /// <param name="fields">List of fields to be retrieved. One of them should have the <see cref="HistorizationProcessing{T}"/></param>
        /// <returns>List of history rows.</returns>
        public List<HistoryRow> GetHistoryValues(string instanceName, HistoryTimeRange timeRange, InstanceTimeSerieParameters instanceTimeSerieParameters, List<IFieldDefinition> fields)
        {
            return [];
        }

        /// <summary>
        /// Gets instance statistic values historized between 2 timestamps.
        /// </summary>
        /// <param name="instanceName">The instance name.</param>
        /// <param name="timeRange">Query containing time range parameters.</param>
        /// <param name="instanceTimeSerieParameters">Parameters defining the time serie.</param>
        /// <param name="fields">List of fields to be retrieved. One of them should have the <see cref="HistorizationProcessing{T}"/></param>
        /// <returns>List of history rows.</returns>
        public List<HistoryStatisticRow> GetHistoryStatistics(string instanceName, HistoryStatisticTimeRange timeRange, InstanceTimeSerieParameters instanceTimeSerieParameters, List<HistoryStatisticField> fields)
        {
            return [];
        }

        /// <summary>
        /// Disposes the instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the instance.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // Do nothing
        }
    }
}
