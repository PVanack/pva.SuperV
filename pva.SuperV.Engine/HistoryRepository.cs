
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Engine.Processing;
using System.Text.Json.Serialization;

namespace pva.SuperV.Engine
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
        public string Name { get; set; } = IdentifierValidation.ValidateIdentifier("history repository", name);

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
        public string HistoryStorageId { get; set; }

        internal void UpsertRepository(string projectName, IHistoryStorageEngine historyStorageEngine)
        {
            HistoryStorageId = historyStorageEngine?.UpsertRepository(projectName, this);
        }

        internal string? UpsertClassTimeSerie<T>(string projectName, string className, HistorizationProcessing<T> historizationProcessing)
        {
            if (HistoryStorageEngine is null)
            {
                throw new NoHistoryStorageEngineException();
            }
            return HistoryStorageEngine.UpsertClassTimeSerie(HistoryStorageId, projectName, className, historizationProcessing);
        }

        public void HistorizeValues(string classTimeSerieId, IInstance instance, DateTime dateTime, List<IField> fieldsToHistorize)
        {
            HistoryStorageEngine?.HistorizeValues(HistoryStorageId, classTimeSerieId, instance.Name!, dateTime, fieldsToHistorize);
        }
    }
}