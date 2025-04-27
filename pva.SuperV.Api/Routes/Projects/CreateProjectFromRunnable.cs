using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Projects;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class CreateProjectFromRunnable
    {
        internal static async Task<Results<Created<ProjectModel>, BadRequest<string>>>
            Handle(IProjectService projectService, string runnableProjectId)
        {
            try
            {
                ProjectModel createdProject = await projectService.CreateProjectFromRunnableAsync(runnableProjectId);
                return TypedResults.Created<ProjectModel>($"/projects/{createdProject.Id}", createdProject);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}