using pva.SuperV.Engine.HistoryStorage;

namespace pva.SuperV.Model.HistoryRepositories
{
    public static class HistoryRepositoryMapper
    {
        public static HistoryRepositoryModel ToDto(HistoryRepository repository)
            => new(repository.Name);
        public static HistoryRepository FromDto(HistoryRepositoryModel repositoryModel)
            => new(repositoryModel.Name);
    }
}
