using pva.SuperV.Model.HistoryRepositories;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Services
{
    public class HistoryRepositoryService(HttpClient httpClient) : IHistoryRepositoryService
    {
        private const string baseUri = "/history-repositories";

        public async Task<HistoryRepositoryModel> CreateHistoryRepositoryAsync(string projectId, HistoryRepositoryModel historyRepositoryCreateRequest)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync($"{baseUri}/{projectId}", historyRepositoryCreateRequest)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    HistoryRepositoryModel? historyRepository = await result.Content.ReadFromJsonAsync<HistoryRepositoryModel>();
                    return historyRepository ?? throw new ApiException("No history repository in response");
                }

                throw new ApiException(result.StatusCode, result.Content);
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async ValueTask DeleteHistoryRepositoryAsync(string projectId, string historyRepositoryName)
        {
            try
            {
                var result = await httpClient.DeleteAsync($"{baseUri}/{projectId}/{historyRepositoryName}")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    await ValueTask.CompletedTask;
                    return;
                }

                throw new ApiException(result.StatusCode, result.Content);
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<List<HistoryRepositoryModel>> GetHistoryRepositoriesAsync(string projectId)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<HistoryRepositoryModel>? historyRepositories = await result.Content.ReadFromJsonAsync<List<HistoryRepositoryModel>>();
                    return historyRepositories ?? throw new ApiException("No history repository in response");
                }

                throw new ApiException(result.StatusCode, result.Content);
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<HistoryRepositoryModel> GetHistoryRepositoryAsync(string projectId, string historyRepositoryName)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}/{historyRepositoryName}")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HistoryRepositoryModel? historyRepository = await result.Content.ReadFromJsonAsync<HistoryRepositoryModel>();
                    return historyRepository ?? throw new ApiException("No history repository in response");
                }

                throw new ApiException(result.StatusCode, result.Content);
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<HistoryRepositoryModel> UpdateHistoryRepositoryAsync(string projectId, string historyRepositoryName, HistoryRepositoryModel historyRepositoryUpdateRequest)
        {
            try
            {
                var result = await httpClient.PutAsJsonAsync($"{baseUri}/{projectId}/{historyRepositoryName}", historyRepositoryUpdateRequest)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HistoryRepositoryModel? historyRepository = await result.Content.ReadFromJsonAsync<HistoryRepositoryModel>();
                    return historyRepository ?? throw new ApiException("No history repositories in response");
                }

                throw new ApiException(result.StatusCode, result.Content);
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }
    }
}
