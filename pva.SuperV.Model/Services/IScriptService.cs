using pva.SuperV.Model.FieldProcessings;

namespace pva.SuperV.Model.Services
{
    public interface IScriptService
    {
        Task<ScriptDefinitionModel> CreateScriptAsync(string projectId, ScriptDefinitionModel createRequest);
        ValueTask DeleteScriptAsync(string projectId, string scriptName);
        Task<ScriptDefinitionModel> GetScriptAsync(string projectId, string scriptName);
        Task<List<ScriptDefinitionModel>> GetScriptsAsync(string projectId);
        Task<ScriptDefinitionModel> UpdateScriptAsync(string projectId, string scriptName, ScriptDefinitionModel updateRequest);
    }
}
