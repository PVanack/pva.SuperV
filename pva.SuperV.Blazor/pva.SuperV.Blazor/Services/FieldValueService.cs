using pva.SuperV.Model.Instances;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Services
{
    public class FieldValueService(HttpClient httpClient) : IFieldValueService
    {
        private const string baseUri = "/instances";
        public async Task<FieldModel> GetFieldAsync(string projectId, string instanceName, string fieldName)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}/{instanceName}/{fieldName}/value")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    FieldModel? field = await result.Content.ReadFromJsonAsync<FieldModel>();
                    return field ?? throw new ApiException("No field in response");
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

        public async Task<FieldValueModel> UpdateFieldValueAsync(string projectId, string instanceName, string fieldName, FieldValueModel value)
        {
            try
            {
                var result = await httpClient.PutAsJsonAsync($"{baseUri}/{projectId}/{instanceName}/{fieldName}/value", value)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    FieldValueModel? field = await result.Content.ReadFromJsonAsync<FieldValueModel>();
                    return field ?? throw new ApiException("No field value in response");
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
