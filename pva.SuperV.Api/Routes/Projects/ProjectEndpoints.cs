using Microsoft.AspNetCore.Mvc;
using pva.SuperV.Api.Services.Projects;
using pva.SuperV.Model;
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
                (IProjectService projectService)
                    => GetProjects.Handle(projectService))
                .WithName("GetProjects")
                .WithDisplayName("GetProjects")
                .WithSummary("Gets the list of available projects")
                .WithDescription("Gets the list of available projects")
                .Produces<List<ProjectModel>>(StatusCodes.Status200OK);

            projectsApi.MapPost("/search",
                (IProjectService projectService, [FromBody] ProjectPagedSearchRequest search)
                    => SearchProjects.Handle(projectService, search))
                .WithName("SearchProjects")
                .WithDisplayName("SearchProjects")
                .WithSummary("Search available projects by page")
                .WithDescription("Search available projects by page")
                .Produces<PagedSearchResult<ProjectModel>>(StatusCodes.Status200OK);

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
                .WithName("CreateBlankProject")
                .WithDisplayName("CreateBlankProjectFromRunnable")
                .WithSummary("Creates a blank WIP project")
                .WithDescription("Creates a blank WIP project")
                .Produces<ProjectModel>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status404NotFound);

            projectsApi.MapPost("/create/{runnableProjectId}",
                (IProjectService projectService,
                [Description(" ID of runnable project")] string runnableProjectId)
                    => CreateProjectFromRunnable.Handle(projectService, runnableProjectId))
                .WithName("CreateProjectFromRunnable")
                .WithDisplayName("CreateProjectFromRunnable")
                .WithSummary("Creates a WIP project from a runnable project")
                .WithDescription("Creates a WIP project from a runnable project")
                .Produces<ProjectModel>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status404NotFound);

            projectsApi.MapPut("/{projectId}",
                (IProjectService projectService,
                [Description(" ID of project")] string projectId,
                [Description("Project update request")][FromBody] UpdateProjectRequest updateProjectRequest)
                    => UpdateProject.Handle(projectService, projectId, updateProjectRequest))
                .WithName("UpdateProject")
                .WithDisplayName("UpdateProject")
                .WithSummary("Updates a project")
                .WithDescription("Updates a project")
                .Produces<ProjectModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound);

            projectsApi.MapPost("/{wipProjectId}/build",
                async (IProjectService projectService,
                        [Description("ID of WIP project")] string wipProjectId)
                    => await BuildProject.Handle(projectService, wipProjectId))
                .WithName("BuildProject")
                .WithSummary("Build a runnable project from a WIP project")
                .WithDescription("Build a runnable project from a WIP project")
                .Produces<ProjectModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            projectsApi.MapGet("/{projectId}/definitions",
                async (IProjectService projectService,
                        [Description("ID of project")] string projectId)
                    => await SaveProjectDefinitions.Handle(projectService, projectId))
                .WithName("SaveProjectDefinitions")
                .WithSummary("Saves the definitions of project")
                .WithDescription("Saves the definitions of project")
                .Produces<string>(StatusCodes.Status200OK, "application/json")
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            projectsApi.MapPost("/load-from-definitions",
                (IProjectService projectService,
                [Description("File data containing project definitioons")]
                [FromBody] byte[] fileData)
                    => LoadProjectFromDefinitions.Handle(projectService, fileData))
                .WithName("LoadProjectFromDefinitions")
                .WithSummary("Loads a project from a definition JSON")
                .WithDescription("Loads a project from a definition JSON")
                .Produces<ProjectModel>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status400BadRequest);

            projectsApi.MapGet("/{runnableProjectId}/instances",
                 async (IProjectService projectService,
                        [Description("ID of runnable project")] string runnableProjectId)
                    => await SaveProjectInstances.Handle(projectService, runnableProjectId))
                .WithName("SaveProjectInstances")
                .WithSummary("Saves the instances of runnable project")
                .WithDescription("Saves the instances of a runnable project")
                .Produces<string>(StatusCodes.Status200OK, "application/json")
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            projectsApi.MapPost("/{runnableProjectId}/instances",
                (IProjectService projectService,
                [Description("ID of runnable project")]
                string runnableProjectId,
                [Description("File data containing the instances")]
                [FromBody] byte[] fileData)
                    => LoadProjectInstances.Handle(projectService, runnableProjectId, fileData))
                .WithName("LoadProjectInstances")
                .WithSummary("Loads runnable project instances from a JSON file")
                .WithDescription("Loads runnable project instances from a JSON file")
                .Produces(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            projectsApi.MapDelete("/{projectId}",
                (IProjectService projectService,
                [Description("ID of project")] string projectId)
                    => UnloadProject.Handle(projectService, projectId))
                .WithName("UnloadProject")
                .WithSummary("Unloads a project")
                .WithDescription("Unloads a project")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            return app;
        }
    }
}
