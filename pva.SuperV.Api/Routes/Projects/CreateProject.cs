using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Projects;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Api.Routes.Projects
{
    public static class CreateProject
    {
        internal static Results<Created<ProjectModel>, InternalServerError<string>> Handle(IProjectService projectService, CreateProjectRequest createProjectRequest)
        {
            try
            {
                ProjectModel createdProject = projectService.CreateProject(createProjectRequest);
                return TypedResults.Created<ProjectModel>($"/projects/{createdProject.Id}", createdProject);
            }
            catch (SuperVException e)
            {
                return TypedResults.InternalServerError(e.Message);
            }
        }
    }
}