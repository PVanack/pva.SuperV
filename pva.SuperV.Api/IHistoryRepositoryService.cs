using pva.SuperV.Engine;
using pva.SuperV.Model.HistoryRepositories;

namespace pva.SuperV.Api
{
    public interface IHistoryRepositoryService
    {
        List<HistoryRepositoryModel> GetHistoryRepositories(string projectId);
        HistoryRepositoryModel GetHistoryRepository(string projectId, string historyRepositoryName);
    }
}
