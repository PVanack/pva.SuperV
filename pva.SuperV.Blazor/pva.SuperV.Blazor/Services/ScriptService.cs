using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Services
{
    public class ScriptService(HttpClient httpClient) : IScriptService
    {
        private const string baseUri = "/scripts";
        public async Task<ScriptDefinitionModel> CreateScriptAsync(string projectId, ScriptDefinitionModel createRequest)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync($"{baseUri}/{projectId}", createRequest)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    ScriptDefinitionModel? script = await result.Content.ReadFromJsonAsync<ScriptDefinitionModel>();
                    return script ?? throw new ApiException("No script in response");
                }

                throw new ApiException(result.StatusCode, result.Content);
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async ValueTask DeleteScriptAsync(string projectId, string scriptName)
        {
            try
            {
                var result = await httpClient.DeleteAsync($"{baseUri}/{projectId}/{scriptName}")
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

        public async Task<ScriptDefinitionModel> GetScriptAsync(string projectId, string scriptName)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}/{scriptName}")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ScriptDefinitionModel? script = await result.Content.ReadFromJsonAsync<ScriptDefinitionModel>();
                    return script ?? throw new ApiException("No script in response");
                }

                throw new ApiException(result.StatusCode, result.Content);
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<List<ScriptDefinitionModel>> GetScriptsAsync(string projectId)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}")
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<ScriptDefinitionModel>? scripts = await result.Content.ReadFromJsonAsync<List<ScriptDefinitionModel>>();
                    return scripts ?? throw new ApiException("No scripts in response");
                }

                throw new ApiException(result.StatusCode, result.Content);
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<ScriptDefinitionModel> UpdateScriptAsync(string projectId, string scriptName, ScriptDefinitionModel updateRequest)
        {
            try
            {
                var result = await httpClient.PutAsJsonAsync($"{baseUri}/{projectId}/{scriptName}", updateRequest)
                    ?? throw new ApiException("No content available");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ScriptDefinitionModel? script = await result.Content.ReadFromJsonAsync<ScriptDefinitionModel>();
                    return script ?? throw new ApiException("No script in response");
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
