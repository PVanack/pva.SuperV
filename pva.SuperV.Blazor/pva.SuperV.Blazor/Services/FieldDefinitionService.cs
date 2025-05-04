using pva.SuperV.Model;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Services
{
    public class FieldDefinitionService(HttpClient httpClient) : IFieldDefinitionService
    {
        private const string baseUri = "/fields";
        public async Task<List<FieldDefinitionModel>> CreateFieldsAsync(string projectId, string className, List<FieldDefinitionModel> createRequests)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync($"{baseUri}/{projectId}/{className}", createRequests)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    List<FieldDefinitionModel>? fields = await result.Content.ReadFromJsonAsync<List<FieldDefinitionModel>>();
                    return fields ?? throw new ApiException("No field definition in response");
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

        public async ValueTask DeleteFieldAsync(string projectId, string className, string fieldName)
        {
            try
            {
                var result = await httpClient.DeleteAsync($"{baseUri}/{projectId}/{className}/{fieldName}")
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

        public async Task<FieldDefinitionModel> GetFieldAsync(string projectId, string className, string fieldName)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}/{className}/{fieldName}")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    FieldDefinitionModel? field = await result.Content.ReadFromJsonAsync<FieldDefinitionModel>();
                    return field ?? throw new ApiException("No field definition in response");
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

        public async Task<List<FieldDefinitionModel>> GetFieldsAsync(string projectId, string className)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}/{className}")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<FieldDefinitionModel>? fields = await result.Content.ReadFromJsonAsync<List<FieldDefinitionModel>>();
                    return fields ?? throw new ApiException("No field definitions in response");
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

        public async Task<PagedSearchResult<FieldDefinitionModel>> SearchFieldsAsync(string projectId, string className, FieldDefinitionPagedSearchRequest search)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync($"{baseUri}/{projectId}/{className}/search", search)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    PagedSearchResult<FieldDefinitionModel>? fieldsPagedSearch = await result.Content.ReadFromJsonAsync<PagedSearchResult<FieldDefinitionModel>>();
                    return fieldsPagedSearch ?? throw new ApiException("No field definitions in response");
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

        public async Task<FieldDefinitionModel> UpdateFieldAsync(string projectId, string className, string fieldName, FieldDefinitionModel updateRequest)
        {
            try
            {
                var result = await httpClient.PutAsJsonAsync($"{baseUri}/{projectId}/{className}/{fieldName}", updateRequest)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    FieldDefinitionModel? field = await result.Content.ReadFromJsonAsync<FieldDefinitionModel>();
                    return field ?? throw new ApiException("No field formatter in response");
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
