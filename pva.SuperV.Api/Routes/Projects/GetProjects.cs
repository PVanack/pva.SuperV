using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Projects;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class GetProjects
    {
        internal static async Task<Results<Ok<List<ProjectModel>>, BadRequest<string>>>
            Handle(IProjectService projectService)
        {
            try
            {
                return TypedResults.Ok(await projectService.GetProjectsAsync());
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}
