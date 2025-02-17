using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;

namespace pva.SuperV.Api.Routes.Projects
{
    public static class GetProjects
    {
        public static Results<Ok<List<ProjectModel>>, InternalServerError<string>> Handle(IProjectService projectService)
        {
            try
            {
                return TypedResults.Ok(projectService.GetProjects());
            }
            catch (SuperVException e)
            {
                return TypedResults.InternalServerError<string>(e.Message);
            }
        }
    }
}
