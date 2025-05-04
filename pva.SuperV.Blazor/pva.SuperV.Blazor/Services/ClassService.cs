using pva.SuperV.Model;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Services
{
    public class ClassService(HttpClient httpClient) : IClassService
    {
        private const string baseUri = "/classes";

        public async Task<ClassModel> CreateClassAsync(string projectId, ClassModel createRequest)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync($"{baseUri}/{projectId}", createRequest)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    ClassModel? clazz = await result.Content.ReadFromJsonAsync<ClassModel>();
                    return clazz ?? throw new ApiException("No class in response");
                }
                else
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async ValueTask DeleteClassAsync(string projectId, string className)
        {
            try
            {
                var result = await httpClient.DeleteAsync($"{baseUri}/{projectId}/{className}")
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

        public async Task<ClassModel> GetClassAsync(string projectId, string className)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}/{className}")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ClassModel? clazz = await result.Content.ReadFromJsonAsync<ClassModel>();
                    return clazz ?? throw new ApiException("No class in response");
                }
                else
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<List<ClassModel>> GetClassesAsync(string projectId)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<ClassModel>? classes = await result.Content.ReadFromJsonAsync<List<ClassModel>>();
                    return classes ?? throw new ApiException("No classes in response");
                }
                else
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<PagedSearchResult<ClassModel>> SearchClassesAsync(string projectId, ClassPagedSearchRequest search)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync($"{baseUri}/{projectId}/search", search)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    PagedSearchResult<ClassModel>? classesPagedSearch = await result.Content.ReadFromJsonAsync<PagedSearchResult<ClassModel>>();
                    return classesPagedSearch ?? throw new ApiException("No classes in response");
                }
                else
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<ClassModel> UpdateClassAsync(string projectId, string className, ClassModel updateRequest)
        {
            try
            {
                var result = await httpClient.PutAsJsonAsync($"{baseUri}/{projectId}/{className}", updateRequest)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ClassModel? clazz = await result.Content.ReadFromJsonAsync<ClassModel>();
                    return clazz ?? throw new ApiException("No field formatter in response");
                }
                else
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }
    }
}
