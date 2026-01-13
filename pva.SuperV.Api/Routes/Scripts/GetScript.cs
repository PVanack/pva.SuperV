using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Scripts
{
    internal static class GetScript
    {
        internal static async Task<Results<Ok<ScriptDefinitionModel>, NotFound<string>, BadRequest<string>>>
            Handle(IScriptService scriptService, string projectId, string scriptName)
        {
            try
            {
                return TypedResults.Ok(await scriptService.GetScriptAsync(projectId, scriptName));
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