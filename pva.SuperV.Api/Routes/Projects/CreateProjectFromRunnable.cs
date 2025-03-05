using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Projects;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class CreateProjectFromRunnable
    {
        internal static Results<Created<ProjectModel>, BadRequest<string>> Handle(IProjectService projectService, string runnableProjectId)
        {
            try
            {
                ProjectModel createdProject = projectService.CreateProjectFromRunnable(runnableProjectId);
                return TypedResults.Created<ProjectModel>($"/projects/{createdProject.Id}", createdProject);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}