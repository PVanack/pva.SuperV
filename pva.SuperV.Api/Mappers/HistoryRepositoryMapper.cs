using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Model.HistoryRepositories;

namespace pva.SuperV.Api.Mappers
{
    public static class HistoryRepositoryMapper
    {
        public static HistoryRepositoryModel ToDto(HistoryRepository repository)
            => new(repository.Name);
        public static HistoryRepository FromDto(HistoryRepositoryModel repositoryModel)
            => new(repositoryModel.Name);
    }
}
