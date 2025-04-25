
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class LoadProjectInstances
    {
        internal static async Task<Results<Ok, NotFound<string>, BadRequest<string>>>
            Handle(IProjectService projectService, string projectId, byte[] fileData)
        {
            try
            {
                using StreamReader reader = new(new MemoryStream(fileData), System.Text.Encoding.UTF8);
                await projectService.LoadProjectInstancesAsync(projectId, reader);
                return TypedResults.Ok();
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