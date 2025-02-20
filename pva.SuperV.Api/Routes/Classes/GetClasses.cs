using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Classes;

namespace pva.SuperV.Api.Routes.Classes
{
    public static class GetClasses
    {
        public static Results<Ok<List<ClassModel>>, NotFound<string>, InternalServerError<string>> Handle(IClassService classService, string projectName)
        {
            try
            {
                return TypedResults.Ok(classService.GetClasses(projectName));
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.InternalServerError<string>(e.Message);
            }
        }
    }
}
