using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Classes;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Classes;

namespace pva.SuperV.Api.Routes.Classes
{
    internal static class GetClass
    {
        internal static Results<Ok<ClassModel>, NotFound<string>, BadRequest<string>> Handle(IClassService classService, string projectName, string className)
        {
            try
            {
                return TypedResults.Ok(classService.GetClass(projectName, className));
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
