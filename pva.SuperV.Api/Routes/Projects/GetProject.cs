
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Projects;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Api.Routes.Projects
{
    public static class GetProject
    {
        public static Results<Ok<ProjectModel>, NotFound<string>, BadRequest<string>> Handle(IProjectService projectService, string projectId)
        {
            try
            {
                return TypedResults.Ok(projectService.GetProject(projectId));
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
