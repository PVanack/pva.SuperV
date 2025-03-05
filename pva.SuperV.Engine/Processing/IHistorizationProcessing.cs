using pva.SuperV.Engine.HistoryStorage;

namespace pva.SuperV.Engine.Processing
{
    /// <summary>
    /// Historization processing. Used only to filter in value change processings instances historizing values for upserting the time series in history storage.
    /// </summary>
    public interface IHistorizationProcessing : IFieldValueProcessing
    {
        /// <summary>
        /// History repository
        /// </summary>
        HistoryRepository? HistoryRepository { get; set; }

        /// <summary>
        /// Field providing the timestamp of time serie.
        /// </summary>
        FieldDefinition<DateTime>? TimestampFieldDefinition { get; set; }

        /// <summary>
        /// List of fields whose value is to be historized.
        /// </summary>
        List<IFieldDefinition> FieldsToHistorize { get; }

        /// <summary>
        /// The class time serie ID returned from history storage.
        /// </summary>
        string? ClassTimeSerieId { get; set; }

        /// <summary>
        /// Upserts the time series in history storage.
        /// </summary>
        /// <param name="projectName">Name of project.</param>
        /// <param name="className">Name of class.</param>
        void UpsertInHistoryStorage(string projectName, string className);
    }
}
