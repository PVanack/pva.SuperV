namespace pva.SuperV.Engine.Processing
{
    public interface IHistorizationProcessing
    {
        HistoryRepository? HistoryRepository { get; set; }
        /// <summary>
        /// The class time serie ID returned from history storage.
        /// </summary>
        string? ClassTimeSerieId { get; set; }


        void UpsertInHistoryStorage(string projectName, string className);
    }
}
