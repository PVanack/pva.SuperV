using pva.SuperV.Model;
using pva.SuperV.Model.FieldFormatters;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Services
{
    public class FieldFormatterService(HttpClient httpClient) : IFieldFormatterService
    {
        private const string baseUri = "/field-formatters";

        public async Task<FieldFormatterModel> CreateFieldFormatterAsync(string projectId, FieldFormatterModel fieldFormatterModel)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync($"{baseUri}/{projectId}", fieldFormatterModel)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    FieldFormatterModel? fieldFormatter = await result.Content.ReadFromJsonAsync<FieldFormatterModel>();
                    return fieldFormatter ?? throw new ApiException("No field formatter in response");
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

        public async ValueTask DeleteFieldFormatterAsync(string projectId, string fieldFormatterName)
        {
            try
            {
                var result = await httpClient.DeleteAsync($"{baseUri}/{projectId}/{fieldFormatterName}")
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

        public async Task<FieldFormatterModel> GetFieldFormatterAsync(string projectId, string fieldFormatterName)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}/{fieldFormatterName}")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    FieldFormatterModel? fieldFormatter = await result.Content.ReadFromJsonAsync<FieldFormatterModel>();
                    return fieldFormatter ?? throw new ApiException("No field formatter in response");
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

        public async Task<List<FieldFormatterModel>> GetFieldFormattersAsync(string projectId)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<FieldFormatterModel>? fieldFormatters = await result.Content.ReadFromJsonAsync<List<FieldFormatterModel>>();
                    return fieldFormatters ?? throw new ApiException("No field formatters in response");
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

        public async Task<List<string>> GetFieldFormatterTypesAsync()
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<string>? fieldFormatterTypes = await result.Content.ReadFromJsonAsync<List<string>>();
                    return fieldFormatterTypes ?? throw new ApiException("No field formatter types in response");
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

        public async Task<PagedSearchResult<FieldFormatterModel>> SearchFieldFormattersAsync(string projectId, FieldFormatterPagedSearchRequest search)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync($"{baseUri}/{projectId}/search", search)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    PagedSearchResult<FieldFormatterModel>? fieldFormattersPagedSearch = await result.Content.ReadFromJsonAsync<PagedSearchResult<FieldFormatterModel>>();
                    return fieldFormattersPagedSearch ?? throw new ApiException("No field formatters in response");
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

        public async Task<FieldFormatterModel> UpdateFieldFormatterAsync(string projectId, string fieldFormatterName, FieldFormatterModel fieldFormatterModel)
        {
            try
            {
                var result = await httpClient.PutAsJsonAsync($"{baseUri}/{projectId}/{fieldFormatterName}", fieldFormatterModel)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    FieldFormatterModel? fieldFormatter = await result.Content.ReadFromJsonAsync<FieldFormatterModel>();
                    return fieldFormatter ?? throw new ApiException("No field formatter in response");
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
