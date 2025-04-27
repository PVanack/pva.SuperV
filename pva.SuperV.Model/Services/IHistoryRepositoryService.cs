using pva.SuperV.Model.HistoryRepositories;

namespace pva.SuperV.Model.Services
{
    public interface IHistoryRepositoryService
    {
        Task<HistoryRepositoryModel> CreateHistoryRepositoryAsync(string projectId, HistoryRepositoryModel historyRepositoryCreateRequest);
        ValueTask DeleteHistoryRepositoryAsync(string projectId, string historyRepositoryName);
        Task<List<HistoryRepositoryModel>> GetHistoryRepositoriesAsync(string projectId);
        Task<HistoryRepositoryModel> GetHistoryRepositoryAsync(string projectId, string historyRepositoryName);
        Task<HistoryRepositoryModel> UpdateHistoryRepositoryAsync(string projectId, string historyRepositoryName, HistoryRepositoryModel historyRepositoryUpdateRequest);
    }
}
