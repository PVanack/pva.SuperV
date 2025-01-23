using pva.SuperV.Engine.Processing;

namespace pva.SuperV.Engine.HistoryStorage
{
    public interface IHistoryStorageEngine : IDisposable
    {
        string UpsertRepository(string projectName, HistoryRepository repository);
        void DeleteRepository(string projectName, string repositoryName);
        string UpsertClassTimeSerie<T>(string repositoryStorageId, string projectName, string className, HistorizationProcessing<T> historizationProcessing);
        void HistorizeValues(string repositoryStorageId, string classTimeSerieId, string instanceName, DateTime dateTime, List<IField> fieldsToHistorize);
        List<HistoryRow> GetHistoryValues(string repositoryStorageId, string classTimeSerieId, string instanceName, DateTime from, DateTime to, List<IFieldDefinition> fields);
    }
}
