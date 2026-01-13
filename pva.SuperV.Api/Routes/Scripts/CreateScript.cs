using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Scripts
{
    internal static class CreateScript
    {
        internal static async Task<Results<Created<ScriptDefinitionModel>, NotFound<string>, BadRequest<string>>>
            Handle(IScriptService scriptService, string projectId, ScriptDefinitionModel createRequest)
        {
            try
            {
                ScriptDefinitionModel createdScript = await scriptService.CreateScriptAsync(projectId, createRequest);
                return TypedResults.Created<ScriptDefinitionModel>($"/field-processings/{projectId}/{createdScript.Name}", createdScript);
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