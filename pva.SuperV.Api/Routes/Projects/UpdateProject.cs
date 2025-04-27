using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Projects;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class UpdateProject
    {
        internal static async Task<Results<Ok<ProjectModel>, NotFound<string>, BadRequest<string>>>
            Handle(IProjectService projectService, string projectId, UpdateProjectRequest updateProjectRequest)
        {
            try
            {
                return TypedResults.Ok(await projectService.UpdateProjectAsync(projectId, updateProjectRequest));
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