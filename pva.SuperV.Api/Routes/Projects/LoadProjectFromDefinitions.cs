
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Projects;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class LoadProjectFromDefinitions
    {
        internal static async Task<Results<Created<ProjectModel>, BadRequest<string>>>
            Handle(IProjectService projectService, byte[] fileData)
        {
            try
            {
                using StreamReader reader = new(new MemoryStream(fileData), System.Text.Encoding.UTF8);
                ProjectModel projectModel = await projectService.CreateProjectFromJsonDefinitionAsync(reader);
                return TypedResults.Created($"/projects/{projectModel.Id}", projectModel);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}