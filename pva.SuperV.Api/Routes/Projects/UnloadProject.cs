using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Projects;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Api.Routes.Projects
{
    public static class UnloadProject
    {
        internal static Results<NoContent, NotFound<string>, BadRequest<string>> Handle(IProjectService projectService, string projectId)
        {
            try
            {
                projectService.UnloadProject(projectId);
                return TypedResults.NoContent();
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound<string>(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}