using Microsoft.AspNetCore.Mvc;
using pva.SuperV.Model;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.Services;
using System.ComponentModel;

namespace pva.SuperV.Api.Routes.Classes
{
    public static class ClassEndpoints
    {
        public static WebApplication MapClassEndpoints(this WebApplication app)
        {
            RouteGroupBuilder classesApi = app.MapGroup("/classes");
            classesApi.MapGet("/{projectId}",
                async (IClassService classService,
                [Description("ID of project")] string projectId)
                    => await GetClasses.Handle(classService, projectId))
                .WithName("GetClasses")
                .WithDisplayName("GetClasses")
                .WithSummary("Gets the list of available classes in a project")
                .WithDescription("Gets the list of available classes in a project")
                .Produces<List<ClassModel>>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            classesApi.MapPost("/{projectId}/search",
                async (IClassService classService,
                [Description("ID of project")] string projectId,
                [FromBody] ClassPagedSearchRequest search)
                    => await SearchClasses.Handle(classService, projectId, search))
                .WithName("SearchClasses")
                .WithDisplayName("SearchClasses")
                .WithSummary("Searches the list of available classes in a project")
                .WithDescription("Searches the list of available classes in a project")
                .Produces<PagedSearchResult<ClassModel>>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            classesApi.MapGet("/{projectId}/{className}",
                async (IClassService classService,
                [Description("ID of project")] string projectId,
                [Description("Name of class")] string className)
                    => await GetClass.Handle(classService, projectId, className))
                .WithName("GetClass")
                .WithDisplayName("GetClass")
                .WithSummary("Gets a class of a project by its name")
                .WithDescription("Gets a class of a project by its name")
                .Produces<ClassModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            classesApi.MapPost("/{wipProjectId}",
                async (IClassService classService,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Class creation request")][FromBody] ClassModel createRequest)
                    => await CreateClass.Handle(classService, wipProjectId, createRequest))
                .WithName("CreateClass")
                .WithDisplayName("CreateClass")
                .WithSummary("Creates a class in a WIP project")
                .WithDescription("Creates a class in a WIP project")
                .Produces<ClassModel>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            classesApi.MapPut("/{wipProjectId}/{className}",
                async (IClassService classService,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Name of class to be updated")] string className,
                [Description("Class update request")][FromBody] ClassModel createRequest)
                    => await UpdateClass.Handle(classService, wipProjectId, className, createRequest))
                .WithName("UpdateClass")
                .WithDisplayName("UpdateClass")
                .WithSummary("Updates a class in a WIP project")
                .WithDescription("Updates a class in a WIP project")
                .Produces<ClassModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            classesApi.MapDelete("/{wipProjectId}/{className}",
                async (IClassService classService,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Name of class")] string className)
                    => await DeleteClass.Handle(classService, wipProjectId, className))
                .WithName("DeleteClass")
                .WithDisplayName("DeleteClass")
                .WithSummary("Deletes a class of a project by its name")
                .WithDescription("Deletes a class of a WIP project by its name")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            return app;
        }
    }
}
