using pva.SuperV.Engine;
using pva.SuperV.Model.HistoryRepositories;

namespace pva.SuperV.Api
{
    public interface IHistoryRepositoryService
    {
        HistoryRepositoryModel CreateHistoryRepository(string projectId, HistoryRepositoryModel historyRepositoryCreateRequest);
        List<HistoryRepositoryModel> GetHistoryRepositories(string projectId);
        HistoryRepositoryModel GetHistoryRepository(string projectId, string historyRepositoryName);
    }
}
