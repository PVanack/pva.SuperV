using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Classes
{
    internal static class GetClass
    {
        internal static async Task<Results<Ok<ClassModel>, NotFound<string>, BadRequest<string>>>
            Handle(IClassService classService, string projectName, string className)
        {
            try
            {
                return TypedResults.Ok(await classService.GetClassAsync(projectName, className));
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
