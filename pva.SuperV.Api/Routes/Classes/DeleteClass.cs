
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Classes
{
    internal static class DeleteClass
    {
        internal static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>>
            Handle(IClassService classService, string projectId, string className)
        {
            try
            {
                await classService.DeleteClassAsync(projectId, className);
                return TypedResults.NoContent();
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound<string>(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest<string>(e.Message);
            }
        }
    }
}