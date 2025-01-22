using pva.SuperV.Engine.Processing;

namespace pva.SuperV.Engine.HistoryStorage
{
    public interface IHistoryStorageEngine
    {
        string? UpsertRepository(string projectName, HistoryRepository repository);
        void DeleteRepository(string projectName, string repositoryName);
        string UpsertClassTimeSerie<T>(string projectName, string className, HistorizationProcessing<T> historizationProcessing);
        void HistorizeValues(string? _classTimeSerieId, string instanceName, DateTime dateTime, List<IField> fieldsToHistorize);
    }
}
