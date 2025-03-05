
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Projects;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class SaveProjectDefinitions
    {
        internal static async Task<Results<FileStreamHttpResult, NotFound<string>, BadRequest<string>, InternalServerError<string>>> Handle(IProjectService projectService, string projectId)
        {
            try
            {
                StreamReader? stream = await projectService.GetProjectDefinitionsAsync(projectId);
                return stream is not null
                    ? TypedResults.Stream(stream.BaseStream, "application/json")
                    : TypedResults.InternalServerError("Project definitions stream is null");
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