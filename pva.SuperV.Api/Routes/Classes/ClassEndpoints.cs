using Microsoft.AspNetCore.Mvc;
using pva.SuperV.Engine;
using pva.SuperV.Model.Classes;
using System.ComponentModel;

namespace pva.SuperV.Api.Routes.Classes
{
    public static class ClassEndpoints
    {
        public static WebApplication MapClassEndpoints(this WebApplication app)
        {
            RouteGroupBuilder projectsApi = app.MapGroup("/classes");
            projectsApi.MapGet("/{projectId}",
                (IClassService classService,
                [Description("ID of project")] string projectId)
                    => GetClasses.Handle(classService, projectId))
                .WithName("GetClasses")
                .WithDisplayName("GetClasses")
                .WithSummary("Gets the list of available classes in a project")
                .WithDescription("Gets the list of available classes in a project")
                .Produces<List<ClassModel>>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound);

            projectsApi.MapGet("/{projectId}/{className}",
                (IClassService classService,
                [Description("ID of project")] string projectId,
                [Description("Name of class")] string className)
                    => GetClass.Handle(classService, projectId, className))
                .WithName("GetClass")
                .WithDisplayName("GetClass")
                .WithSummary("Gets a class of a project by its name")
                .WithDescription("Gets a class of a project by its name")
                .Produces<ClassModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound);

            projectsApi.MapPost("/{projectId}",
                (IClassService classService,
                [Description("ID of project")] string projectId,
                [Description("Class creation request")][FromBody] ClassModel createRequest)
                    => CreateClass.Handle(classService, projectId, createRequest))
                .WithName("CreateClass")
                .WithDisplayName("CreateClass")
                .WithSummary("Creates a class in a project")
                .WithDescription("Creates a class in a project")
                .Produces<ClassModel>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status404NotFound);

            projectsApi.MapDelete("/{projectId}/{className}",
                (IClassService classService,
                [Description("ID of project")] string projectId,
                [Description("Name of class")] string className)
                    => DeleteClass.Handle(classService, projectId, className))
                .WithName("DeleteClass")
                .WithDisplayName("DeleteClass")
                .WithSummary("Deletes a class of a project by its name")
                .WithDescription("Deletes a class of a project by its name")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<string>(StatusCodes.Status404NotFound);

            return app;
        }
    }
}
