using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Projects;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class UpdateProject
    {
        internal static Results<Ok<ProjectModel>, NotFound<string>, BadRequest<string>> Handle(IProjectService projectService, string projectId, UpdateProjectRequest updateProjectRequest)
        {
            try
            {
                return TypedResults.Ok(projectService.UpdateProject(projectId, updateProjectRequest));
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