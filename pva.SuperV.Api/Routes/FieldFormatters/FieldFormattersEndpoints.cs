using Microsoft.AspNetCore.Mvc;
using pva.SuperV.Api.Services.FieldFormatters;
using pva.SuperV.Model.FieldFormatters;
using System.ComponentModel;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    public static class FieldFormattersEndpoints
    {
        public static WebApplication MapFieldFormatterEndpoints(this WebApplication app)
        {
            RouteGroupBuilder projectsApi = app.MapGroup("/field-formatters");
            projectsApi.MapGet("/",
                (IFieldFormatterService fieldFormatterService) =>
                    GetFieldFormatterTypes.Handle(fieldFormatterService))
                .WithName("GetFieldFormatterTypes")
                .WithDisplayName("GetFieldFormatterTypes")
                .WithSummary("Gets the list of available field formatter types")
                .WithDescription("Gets the list of available field formatter types")
                .Produces<List<string>>(StatusCodes.Status200OK);

            projectsApi.MapGet("/{projectId}",
                (IFieldFormatterService fieldFormatterService,
                [Description("ID of project")] string projectId)
                    => GetFieldFormatters.Handle(fieldFormatterService, projectId))
                .WithName("GetFieldFormatters")
                .WithDisplayName("GetFieldFormatters")
                .WithSummary("Gets the list of field formatters of project")
                .WithDescription("Gets the list of field formatters of project")
                .Produces<List<FieldFormatterModel>>(StatusCodes.Status200OK);

            projectsApi.MapGet("/{projectId}/{fieldFormatterName}",
                (IFieldFormatterService fieldFormatterService,
                [Description("ID of project")] string projectId,
                [Description("Name of field formatter")] string fieldFormatterName)
                    => GetFieldFormatter.Handle(fieldFormatterService, projectId, fieldFormatterName))
                .WithName("GetFieldFormatter")
                .WithDisplayName("GetFieldFormatter")
                .WithSummary("Gets a field formatter of project")
                .WithDescription("Gets a field formatter of project")
                .Produces<FieldFormatterModel>(StatusCodes.Status200OK);

            projectsApi.MapPost("/{projectId}",
                (IFieldFormatterService fieldFormatterService, HttpContext context,
                [Description("ID of project")] string projectId,
                [Description("Field formatter creation request")][FromBody] CreateFieldFormatterRequest createRequest)
                    => CreateFieldFormatter.Handle(fieldFormatterService, projectId, createRequest))
                .WithName("CreateFieldFormatter")
                .WithDisplayName("CreateFieldFormatter")
                .WithSummary("Creates a field formatter of project")
                .WithDescription("Creates a field formatter of project")
                .Produces<FieldFormatterModel>(StatusCodes.Status201Created);

            projectsApi.MapDelete("/{projectId}/{fieldFormatterName}",
                (IFieldFormatterService fieldFormatterService,
                [Description("ID of project")] string projectId,
                [Description("Name of field formatter")] string fieldFormatterName)
                    => DeleteFieldFormatter.Handle(fieldFormatterService, projectId, fieldFormatterName))
                .WithName("DeleteFieldFormatter")
                .WithDisplayName("DeleteFieldFormatter")
                .WithSummary("Deletes a field formatter of project")
                .WithDescription("Deletes a field formatter of project")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            return app;
        }
    }
}
