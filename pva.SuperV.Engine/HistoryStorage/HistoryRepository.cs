using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.Processing;
using System.Text.Json.Serialization;

namespace pva.SuperV.Engine.HistoryStorage
{
    /// <summary>
    /// History repository to store history of instance values.
    /// </summary>
    public class HistoryRepository(string name)
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; } = IdentifierValidation.ValidateIdentifier("history repository", name);

        /// <summary>
        /// Gets or sets the history storage engine used to store values.
        /// </summary>
        /// <value>
        /// The history storage engine.
        /// </value>
        [JsonIgnore]
        public IHistoryStorageEngine? HistoryStorageEngine { get; set; }

        /// <summary>
        /// Gets of sets the history storage ID.
        /// </summary>
        /// <value> The history storage ID</value>
        public string? HistoryStorageId { get; set; }

        /// <summary>
        /// Upserts the repository in storage engine.
        /// </summary>
        /// <param name="projectName">Name of project.</param>
        /// <param name="historyStorageEngine">History storage engine.</param>
        internal void UpsertRepository(string projectName, IHistoryStorageEngine historyStorageEngine) =>
            HistoryStorageId = historyStorageEngine.UpsertRepository(projectName, this);

        /// <summary>
        /// Upserts a time series in storage engine.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="projectName">Name of project.</param>
        /// <param name="className">Name of class.</param>
        /// <param name="historizationProcessing">History processing for which the time series is to be created.</param>
        /// <returns></returns>
        /// <exception cref="NoHistoryStorageEngineException"></exception>
        internal string UpsertClassTimeSerie<T>(string projectName, string className, HistorizationProcessing<T> historizationProcessing)
        {
            if (HistoryStorageEngine is null)
            {
                throw new NoHistoryStorageEngineException();
            }
            return HistoryStorageEngine.UpsertClassTimeSerie(HistoryStorageId!, projectName, className, historizationProcessing);
        }

        /// <summary>
        /// Historize instance values in storage engine
        /// </summary>
        /// <param name="classTimeSerieId">The time series ID.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="timestamp">the timestamp of the values</param>
        /// <param name="fieldsToHistorize">List of fields to be historized.</param>
        public void HistorizeValues(string classTimeSerieId, IInstance instance, DateTime timestamp, QualityLevel? quality, List<IField> fieldsToHistorize)
        {
            HistoryStorageEngine?.HistorizeValues(HistoryStorageId!, classTimeSerieId, instance.Name, timestamp, quality, fieldsToHistorize);
        }
    }
}