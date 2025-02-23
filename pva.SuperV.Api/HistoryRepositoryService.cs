using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Model.HistoryRepositories;

namespace pva.SuperV.Api
{
    public class HistoryRepositoryService: BaseService, IHistoryRepositoryService
    {
        public List<HistoryRepositoryModel> GetHistoryRepositories(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            return project.HistoryRepositories.Values
                .Select(HistoryRepositoryMapper.ToDto)
                .ToList();
        }

        public HistoryRepositoryModel GetHistoryRepository(string projectId, string historyRepositoryName)
        {
            Project project = GetProjectEntity(projectId);
            if (project.HistoryRepositories.TryGetValue(historyRepositoryName, out HistoryRepository? historyRepository)) {
                return HistoryRepositoryMapper.ToDto(historyRepository);
            }
            throw new UnknownEntityException("History repository", historyRepositoryName);
        }
    }
}
