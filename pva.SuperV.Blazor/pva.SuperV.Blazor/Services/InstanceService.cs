using pva.SuperV.Model;
using pva.SuperV.Model.Instances;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Services
{
    public class InstanceService(HttpClient httpClient) : IInstanceService
    {
        private const string baseUri = "/instances";
        public async Task<InstanceModel> CreateInstanceAsync(string projectId, InstanceModel createRequest, bool addToRunningInstances = true)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync($"{baseUri}/{projectId}?addToRunningInstances={addToRunningInstances}", createRequest)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    InstanceModel? instance = await result.Content.ReadFromJsonAsync<InstanceModel>();
                    return instance ?? throw new ApiException("No instance in response");
                }

                throw new ApiException(result.StatusCode, result.Content);
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async ValueTask DeleteInstanceAsync(string projectId, string instanceName)
        {
            try
            {
                var result = await httpClient.DeleteAsync($"{baseUri}/{projectId}/{instanceName}")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<InstanceModel> GetInstanceAsync(string projectId, string instanceName)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}/{instanceName}")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    InstanceModel? instance = await result.Content.ReadFromJsonAsync<InstanceModel>();
                    return instance ?? throw new ApiException("No instance in response");
                }

                throw new ApiException(result.StatusCode, result.Content);
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<List<InstanceModel>> GetInstancesAsync(string projectId)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<InstanceModel>? instances = await result.Content.ReadFromJsonAsync<List<InstanceModel>>();
                    return instances ?? throw new ApiException("No instances in response");
                }

                throw new ApiException(result.StatusCode, result.Content);
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<PagedSearchResult<InstanceModel>> SearchInstancesAsync(string projectId, InstancePagedSearchRequest search)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync($"{baseUri}/{projectId}/search", search)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    PagedSearchResult<InstanceModel>? instancesPagedSearch = await result.Content.ReadFromJsonAsync<PagedSearchResult<InstanceModel>>();
                    return instancesPagedSearch ?? throw new ApiException("No instances in response");
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
