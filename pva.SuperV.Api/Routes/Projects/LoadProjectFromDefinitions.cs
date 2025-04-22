
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Projects;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class LoadProjectFromDefinitions
    {
        internal static Results<Created<ProjectModel>, BadRequest<string>> Handle(IProjectService projectService, byte[] fileData)
        {
            try
            {
                using StreamReader reader = new(new MemoryStream(fileData), System.Text.Encoding.UTF8);
                ProjectModel projectModel = projectService.CreateProjectFromJsonDefinition(reader);
                return TypedResults.Created($"/projects/{projectModel.Id}", projectModel);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}