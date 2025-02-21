using Microsoft.AspNetCore.Mvc;
using pva.SuperV.Model.Projects;
using System.ComponentModel;

namespace pva.SuperV.Api.Routes.Projects
{
    public static class ProjectEndpoints
    {
        public static WebApplication MapProjectEndpoints(this WebApplication app)
        {
            RouteGroupBuilder projectsApi = app.MapGroup("/projects");
            projectsApi.MapGet("/",
                (IProjectService projectService) =>
                    GetProjects.Handle(projectService))
                .WithName("GetProjects")
                .WithDisplayName("GetProjects")
                .WithSummary("Gets the list of available projects")
                .WithDescription("Gets the list of available projects")
                .Produces<List<ProjectModel>>(StatusCodes.Status200OK);

            projectsApi.MapGet("/{projectId}",
                (IProjectService projectService,
                [Description("ID of project")] string projectId)
                    => GetProject.Handle(projectService, projectId))
                .WithName("GetProject")
                .WithDisplayName("GetProject")
                .WithSummary("Gets a project by its ID")
                .WithDescription("Gets a project by its ID")
                .Produces<ProjectModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound);

            projectsApi.MapPost("/create",
                (IProjectService projectService,
                [Description("Project creation request")][FromBody] CreateProjectRequest createProjectRequest)
                    => CreateProject.Handle(projectService, createProjectRequest))
                .WithName("CreateProject")
                .WithDisplayName("CreateProject")
                .WithSummary("Creates a blank WIP project")
                .WithDescription("Creates a blank WIP project")
                .Produces<ProjectModel>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status404NotFound);

            projectsApi.MapPost("/{projectId}/build",
                (IProjectService projectService,
                [Description("ID of project")] string projectId)
                    => BuildProject.Handle(projectService, projectId))
                .WithName("BuildProject")
                .WithSummary("Build a runnable project from a WIP project")
                .WithDescription("Build a runnable project from a WIP project")
                .Produces<ProjectModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            projectsApi.MapGet("/{projectId}/definitions",
                (IProjectService projectService,
                [Description("ID of project")] string projectId)
                    => SaveProjectDefinitions.Handle(projectService, projectId))
                .WithName("SaveProjectDefinitions")
                .WithSummary("Saves the definitions of project to a stream writer")
                .WithDescription("Saves the definitions of project to a stream writer")
                .Produces<string>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            projectsApi.MapPost("/load-from-definitions",
                (IProjectService projectService,
                HttpRequest request)
                    => LoadProjectFromDefinitions.Handle(projectService, request))
                .WithName("LoadProjectFromDefinitions")
                .WithSummary("Loads a project from a definition JSON")
                .WithDescription("Loads a project from a definition JSON")
                .Accepts<IFormFile>("multipart/form-data")
                .Produces<ProjectModel>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            projectsApi.MapDelete("/{projectId}",
                (IProjectService projectService,
                [Description("ID of project")] string projectId)
                    => UnloadProject.Handle(projectService, projectId))
                .WithName("UnloadProject")
                .WithSummary("Unloads a project")
                .WithDescription("Unloads a project")
                .Produces(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);
            return app;
        }
    }
}
