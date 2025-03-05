using pva.SuperV.Model.HistoryRepositories;

namespace pva.SuperV.Api.Services.HistoryRepositories
{
    public interface IHistoryRepositoryService
    {
        HistoryRepositoryModel CreateHistoryRepository(string projectId, HistoryRepositoryModel historyRepositoryCreateRequest);
        void DeleteHistoryRepository(string projectId, string historyRepositoryName);
        List<HistoryRepositoryModel> GetHistoryRepositories(string projectId);
        HistoryRepositoryModel GetHistoryRepository(string projectId, string historyRepositoryName);
    }
}
