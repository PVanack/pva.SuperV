using Microsoft.AspNetCore.Mvc;
using pva.SuperV.Model;
using pva.SuperV.Model.FieldFormatters;
using pva.SuperV.Model.Services;
using System.ComponentModel;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    public static class FieldFormattersEndpoints
    {
        public static WebApplication MapFieldFormatterEndpoints(this WebApplication app)
        {
            RouteGroupBuilder fieldFormattersApi = app.MapGroup("/field-formatters");
            fieldFormattersApi.MapGet("/",
                async (IFieldFormatterService fieldFormatterService)
                => await GetFieldFormatterTypes.Handle(fieldFormatterService))
                .WithName("GetFieldFormatterTypes")
                .WithDisplayName("GetFieldFormatterTypes")
                .WithSummary("Gets the list of available field formatter types")
                .WithDescription("Gets the list of available field formatter types")
                .Produces<List<string>>(StatusCodes.Status200OK);

            fieldFormattersApi.MapGet("/{projectId}",
                async (IFieldFormatterService fieldFormatterService,
                [Description("ID of project")] string projectId)
                    => await GetFieldFormatters.Handle(fieldFormatterService, projectId))
                .WithName("GetFieldFormatters")
                .WithDisplayName("GetFieldFormatters")
                .WithSummary("Gets the list of field formatters of project")
                .WithDescription("Gets the list of field formatters of project")
                .Produces<List<FieldFormatterModel>>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound);

            fieldFormattersApi.MapPost("/{projectId}/search",
                async (IFieldFormatterService fieldFormatterService,
                [Description("ID of project")] string projectId,
                [FromBody] FieldFormatterPagedSearchRequest search)
                => await SearchFieldFormatters.Handle(fieldFormatterService, projectId, search))
                .WithName("SearchFieldFormatterTypes")
                .WithDisplayName("SearchFieldFormatterTypes")
                .WithSummary("Searches the list of available field formatter types")
                .WithDescription("Searches the list of available field formatter types")
                .Produces<PagedSearchResult<FieldFormatterModel>>(StatusCodes.Status200OK);

            fieldFormattersApi.MapGet("/{projectId}/{fieldFormatterName}",
                async (IFieldFormatterService fieldFormatterService,
                [Description("ID of project")] string projectId,
                [Description("Name of field formatter")] string fieldFormatterName)
                    => await GetFieldFormatter.Handle(fieldFormatterService, projectId, fieldFormatterName))
                .WithName("GetFieldFormatter")
                .WithDisplayName("GetFieldFormatter")
                .WithSummary("Gets a field formatter of project")
                .WithDescription("Gets a field formatter of project")
                .Produces<FieldFormatterModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldFormattersApi.MapPost("/{wipProjectId}",
                async (IFieldFormatterService fieldFormatterService, HttpContext context,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Field formatter creation request")][FromBody] CreateFieldFormatterRequest createRequest)
                    => await CreateFieldFormatter.Handle(fieldFormatterService, wipProjectId, createRequest))
                .WithName("CreateFieldFormatter")
                .WithDisplayName("CreateFieldFormatter")
                .WithSummary("Creates a field formatter in a WIP project")
                .WithDescription("Creates a field formatter in a WIP project")
                .Produces<FieldFormatterModel>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldFormattersApi.MapPut("/{wipProjectId}/{fieldFormatterName}",
                async (IFieldFormatterService fieldFormatterService, HttpContext context,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Field formatter name")] string fieldFormatterName,
                [Description("Field formatter update model")][FromBody] FieldFormatterModel fieldFormatterModel)
                    => await UpdateFieldFormatter.Handle(fieldFormatterService, wipProjectId, fieldFormatterName, fieldFormatterModel))
                .WithName("UpdateFieldFormatter")
                .WithDisplayName("UpdateFieldFormatter")
                .WithSummary("Updates a field formatter in a WIP project")
                .WithDescription("Updates a field formatter in a WIP project")
                .Produces<FieldFormatterModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldFormattersApi.MapDelete("/{wipProjectId}/{fieldFormatterName}",
                async (IFieldFormatterService fieldFormatterService,
                [Description("ID of WUP project")] string wipProjectId,
                [Description("Name of field formatter")] string fieldFormatterName)
                    => await DeleteFieldFormatter.Handle(fieldFormatterService, wipProjectId, fieldFormatterName))
                .WithName("DeleteFieldFormatter")
                .WithDisplayName("DeleteFieldFormatter")
                .WithSummary("Deletes a field formatter from a WIP project")
                .WithDescription("Deletes a field formatter from a WIP project")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            return app;
        }
    }
}
