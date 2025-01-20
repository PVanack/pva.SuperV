namespace pva.SuperV.Engine
{
    public interface IHistoryStorageEngine
    {
        void CreateRepository(HistoryRepository repository);
        void UpdateRepository(HistoryRepository repository);
        void DeleteRepository(string repositoryName);
        bool RepositoryExists(string repositoryName);
    }
}
