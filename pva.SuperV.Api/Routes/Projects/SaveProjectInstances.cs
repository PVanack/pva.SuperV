
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class SaveProjectInstances
    {
        internal static async Task<Results<FileStreamHttpResult, NotFound<string>, BadRequest<string>, InternalServerError<string>>> Handle(IProjectService projectService, string projectId)
        {
            try
            {
                StreamReader? stream = await projectService.GetProjectInstancesAsync(projectId);
                return stream is not null
                    ? TypedResults.Stream(stream.BaseStream, "application/json")
                    : TypedResults.InternalServerError("Project instances stream is null");
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound(e.Message);
            }
            catch (NonRunnableProjectException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.InternalServerError(e.Message);
            }
        }
    }
}