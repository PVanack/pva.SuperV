using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Projects;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class CreateProject
    {
        internal static async Task<Results<Created<ProjectModel>, BadRequest<string>>>
            Handle(IProjectService projectService, CreateProjectRequest createProjectRequest)
        {
            try
            {
                ProjectModel createdProject = await projectService.CreateProjectAsync(createProjectRequest);
                return TypedResults.Created<ProjectModel>($"/projects/{createdProject.Id}", createdProject);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}