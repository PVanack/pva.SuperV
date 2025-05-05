using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Services
{
    public class FieldProcessingService(HttpClient httpClient) : IFieldProcessingService
    {
        private const string baseUri = "/field-processings";

        public async Task<FieldValueProcessingModel> CreateFieldProcessingAsync(string projectId, string className, string fieldName, FieldValueProcessingModel createRequest)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync($"{baseUri}/{projectId}/{className}/{fieldName}", createRequest)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    FieldValueProcessingModel? fieldProcessing = await result.Content.ReadFromJsonAsync<FieldValueProcessingModel>();
                    return fieldProcessing ?? throw new ApiException("No field processing in response");
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

        public async ValueTask DeleteFieldProcessingAsync(string projectId, string className, string fieldName, string processingName)
        {
            try
            {
                var result = await httpClient.DeleteAsync($"{baseUri}/{projectId}/{className}/{fieldName}/{processingName}")
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

        public async Task<FieldValueProcessingModel> GetFieldProcessingAsync(string projectId, string className, string fieldName, string processingName)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}/{className}/{fieldName}/{processingName}")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    FieldValueProcessingModel? fieldProcessing = await result.Content.ReadFromJsonAsync<FieldValueProcessingModel>();
                    return fieldProcessing ?? throw new ApiException("No field processing in response");
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

        public async Task<List<FieldValueProcessingModel>> GetFieldProcessingsAsync(string projectId, string className, string fieldName)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}/{className}/{fieldName}")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<FieldValueProcessingModel>? fieldProcessings = await result.Content.ReadFromJsonAsync<List<FieldValueProcessingModel>>();
                    return fieldProcessings ?? throw new ApiException("No field processings in response");
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

        public async Task<FieldValueProcessingModel> UpdateFieldProcessingAsync(string projectId, string className, string fieldName, string processingName, FieldValueProcessingModel updateRequest)
        {
            try
            {
                var result = await httpClient.PutAsJsonAsync($"{baseUri}/{projectId}/{className}/{fieldName}/{processingName}", updateRequest)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    FieldValueProcessingModel? fieldProcessing = await result.Content.ReadFromJsonAsync<FieldValueProcessingModel>();
                    return fieldProcessing ?? throw new ApiException("No field processing in response");
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
