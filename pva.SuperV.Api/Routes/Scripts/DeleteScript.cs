using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Scripts
{
    internal static class DeleteScript
    {
        internal static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>>
            Handle(IScriptService scriptService, string projectId, string scriptName)
        {
            try
            {
                await scriptService.DeleteScriptAsync(projectId, scriptName);
                return TypedResults.NoContent();
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound<string>(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}