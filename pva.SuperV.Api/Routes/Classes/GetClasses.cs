using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Classes
{
    internal static class GetClasses
    {
        internal static async Task<Results<Ok<List<ClassModel>>, NotFound<string>, BadRequest<string>>> Handle(IClassService classService, string projectId)
        {
            try
            {
                return TypedResults.Ok(await classService.GetClassesAsync(projectId));
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
