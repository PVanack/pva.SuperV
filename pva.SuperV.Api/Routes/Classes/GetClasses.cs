using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Classes;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Classes;

namespace pva.SuperV.Api.Routes.Classes
{
    public static class GetClasses
    {
        public static Results<Ok<List<ClassModel>>, NotFound<string>, BadRequest<string>> Handle(IClassService classService, string projectId)
        {
            try
            {
                return TypedResults.Ok(classService.GetClasses(projectId));
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
