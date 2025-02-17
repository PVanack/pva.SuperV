
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;

namespace pva.SuperV.Api.Routes.Projects
{
    public static class GetProject
    {
        public static Results<Ok<ProjectModel>, NotFound<string>, InternalServerError<string>> Handle(IProjectService projectService, string projectName)
        {
            try
            {
                return TypedResults.Ok(projectService.GetProject(projectName));
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
