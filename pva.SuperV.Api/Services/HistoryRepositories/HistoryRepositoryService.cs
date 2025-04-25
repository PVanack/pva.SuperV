using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Model.HistoryRepositories;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Services.HistoryRepositories
{
    public class HistoryRepositoryService : BaseService, IHistoryRepositoryService
    {
        public async Task<List<HistoryRepositoryModel>> GetHistoryRepositoriesAsync(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            return await Task.FromResult(project.HistoryRepositories.Values.Select(HistoryRepositoryMapper.ToDto).ToList());
        }

        public async Task<HistoryRepositoryModel> GetHistoryRepositoryAsync(string projectId, string historyRepositoryName)
        {
            if (GetProjectEntity(projectId).HistoryRepositories.TryGetValue(historyRepositoryName, out HistoryRepository? historyRepository))
            {
                return await Task.FromResult(HistoryRepositoryMapper.ToDto(historyRepository));
            }
            return await Task.FromException<HistoryRepositoryModel>(new UnknownEntityException("History repository", historyRepositoryName));
        }

        public async Task<HistoryRepositoryModel> CreateHistoryRepositoryAsync(string projectId, HistoryRepositoryModel historyRepositoryCreateRequest)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                HistoryRepository historyRepository = HistoryRepositoryMapper.FromDto(historyRepositoryCreateRequest);
                wipProject.AddHistoryRepository(historyRepository);
                return await Task.FromResult(HistoryRepositoryMapper.ToDto(historyRepository));
            }
            return await Task.FromException<HistoryRepositoryModel>(new NonWipProjectException(projectId));
        }

        public async Task<HistoryRepositoryModel> UpdateHistoryRepositoryAsync(string projectId, string historyRepositoryName, HistoryRepositoryModel historyRepositoryUpdateRequest)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                HistoryRepository historyRepositoryUpdate = HistoryRepositoryMapper.FromDto(historyRepositoryUpdateRequest);
                wipProject.UpdateHistoryRepository(historyRepositoryName, historyRepositoryUpdate);
                return await Task.FromResult(HistoryRepositoryMapper.ToDto(historyRepositoryUpdate));
            }
            return await Task.FromException<HistoryRepositoryModel>(new NonWipProjectException(projectId));
        }

        public async ValueTask DeleteHistoryRepositoryAsync(string projectId, string historyRepositoryName)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                wipProject.RemoveHistoryRepository(historyRepositoryName);
                await ValueTask.CompletedTask;
                return;
            }
            await ValueTask.FromException(new NonWipProjectException(projectId));
        }
    }
}
