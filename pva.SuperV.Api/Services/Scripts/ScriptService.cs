using pva.SuperV.Api.Exceptions;
using pva.SuperV.Api.Mappers;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.Processing;
using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Services.Scripts
{
    public class ScriptService : BaseService, IScriptService
    {
        private readonly ILogger logger;

        public ScriptService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(this.GetType());
        }

        public async Task<List<ScriptDefinitionModel>> GetScriptsAsync(string projectId)
        {
            logger.LogDebug("Getting scripts of project {ProjectId}",
                projectId);
            return await Task.FromResult(GetProjectEntity(projectId).ScriptDefinitions.Values.Select(field => ScriptDefinitionMapper.ToDto(field)).ToList());
        }

        public async Task<ScriptDefinitionModel> GetScriptAsync(string projectId, string scriptName)
        {
            logger.LogDebug("Getting script {scriptName} of project {ProjectId}",
                scriptName, projectId);
            return GetProjectEntity(projectId).ScriptDefinitions.TryGetValue(scriptName, out ScriptDefinition? script)
                ? await Task.FromResult(ScriptDefinitionMapper.ToDto(script))
                : await Task.FromException<ScriptDefinitionModel>(new UnknownEntityException("Script", scriptName));
        }

        public async Task<ScriptDefinitionModel> CreateScriptAsync(string projectId, ScriptDefinitionModel createRequest)
        {
            logger.LogDebug("Creating script {scriptName} on project {ProjectId}",
                createRequest.Name, projectId);
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                ScriptDefinition scriptDefinition = ScriptDefinitionMapper.FromDto(createRequest);
                wipProject.AddScript(scriptDefinition);
                return await Task.FromResult(ScriptDefinitionMapper.ToDto(scriptDefinition));
            }
            return await Task.FromException<ScriptDefinitionModel>(new NonWipProjectException(projectId));
        }

        public async Task<ScriptDefinitionModel> UpdateScriptAsync(string projectId, string scriptName, ScriptDefinitionModel updateRequest)
        {
            logger.LogDebug("Updating script {scriptName} on project {ProjectId}",
                scriptName, projectId);
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                ScriptDefinition scriptDefinition = ScriptDefinitionMapper.FromDto(updateRequest);
                wipProject.UpdateScript(scriptDefinition);
                return await Task.FromResult(ScriptDefinitionMapper.ToDto(scriptDefinition));
            }
            return await Task.FromException<ScriptDefinitionModel>(new NonWipProjectException(projectId));
        }

        public async ValueTask DeleteScriptAsync(string projectId, string scriptName)
        {
            logger.LogDebug("Deleting script {scriptName} on project {ProjectId}",
                scriptName, projectId);
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                wipProject.RemoveScript(scriptName);
                await ValueTask.CompletedTask;
                return;
            }
            await ValueTask.FromException(new NonWipProjectException(projectId));
        }
    }
}
