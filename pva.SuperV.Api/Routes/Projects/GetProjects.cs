using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Projects;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class GetProjects
    {
        internal static Results<Ok<List<ProjectModel>>, BadRequest<string>> Handle(IProjectService projectService)
        {
            try
            {
                return TypedResults.Ok(projectService.GetProjects());
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}
