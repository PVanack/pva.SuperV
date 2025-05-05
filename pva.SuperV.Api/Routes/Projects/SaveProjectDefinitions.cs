using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class SaveProjectDefinitions
    {
        internal static async Task<IResult> Handle(IProjectService projectService, string projectId)
        {
            try
            {
                Stream? stream = await projectService.GetProjectDefinitionsAsync(projectId);
                if (stream is null)
                {
                    return TypedResults.InternalServerError("Project definitions stream is null");
                }
                MemoryStream memoryStream = new();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                return Results.Stream(memoryStream);
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