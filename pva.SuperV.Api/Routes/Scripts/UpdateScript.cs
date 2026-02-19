using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Scripts
{
    internal static class UpdateScript
    {
        internal static async Task<Results<Ok<ScriptDefinitionModel>, NotFound<string>, BadRequest<string>>>
            Handle(IScriptService scriptService, string projectId, string scriptName, ScriptDefinitionModel updateRequest)
        {
            try
            {
                ScriptDefinitionModel updatedScript = await scriptService.UpdateScriptAsync(projectId, scriptName, updateRequest);
                return TypedResults.Ok<ScriptDefinitionModel>(updatedScript);
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}