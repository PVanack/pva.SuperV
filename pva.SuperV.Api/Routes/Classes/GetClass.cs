using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;

namespace pva.SuperV.Api.Routes.Classes
{
    public static class GetClass
    {
        internal static Results<Ok<ClassModel>, NotFound<string>, InternalServerError<string>> Handle(IProjectService projectService, string projectName, string className)
        {
            try
            {
                return TypedResults.Ok(projectService.GetClass(projectName, className));
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
