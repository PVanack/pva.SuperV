namespace pva.SuperV.Engine.Processing
{
    /// <summary>
    /// Historization processing. Used only to filter in value change processings instances historizing values for upserting the time series in history storage.
    /// </summary>
    public interface IHistorizationProcessing
    {
        /// <summary>
        /// History repository
        /// </summary>
        HistoryRepository? HistoryRepository { get; set; }

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
