using Microsoft.AspNetCore.Mvc;
using pva.SuperV.Model;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.Services;
using System.ComponentModel;

namespace pva.SuperV.Api.Routes.FieldDefinitions
{
    public static class FieldDefinitionEndpoints
    {
        public static WebApplication MapFieldDefinitionEndpoints(this WebApplication app)
        {
            RouteGroupBuilder fieldDefinitionsApi = app.MapGroup("/fields");
            fieldDefinitionsApi.MapGet("/{projectId}/{className}",
                async (IFieldDefinitionService fieldDefinitionService,
                [Description("ID of project")] string projectId,
                [Description("Name of class")] string className)
                    => await GetFieldDefinitions.Handle(fieldDefinitionService, projectId, className))
                .WithName("GetFieldDefinitions")
                .WithDisplayName("GetFieldDefinitions")
                .WithSummary("Gets the list of available fields in a class from a project")
                .WithDescription("Gets the list of available fields in a class from a project")
                .Produces<List<FieldDefinitionModel>>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldDefinitionsApi.MapPost("/{projectId}/{className}/search",
                async (IFieldDefinitionService fieldDefinitionService,
                [Description("ID of project")] string projectId,
                [Description("Name of class")] string className,
                [FromBody] FieldDefinitionPagedSearchRequest search)
                    => await SearchFieldDefinitions.Handle(fieldDefinitionService, projectId, className, search))
                .WithName("SearchFieldDefinitions")
                .WithDisplayName("SearchFieldDefinitions")
                .WithSummary("Searches the list of available fields in a class from a project")
                .WithDescription("Searches the list of available fields in a class from a project")
                .Produces<PagedSearchResult<FieldDefinitionModel>>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldDefinitionsApi.MapGet("/{projectId}/{className}/{fieldName}",
                async (IFieldDefinitionService fieldDefinitionService,
                [Description("ID of project")] string projectId,
                [Description("Name of class")] string className,
                [Description("Name of field")] string fieldName)
                    => await GetFieldDefinition.Handle(fieldDefinitionService, projectId, className, fieldName))
                .WithName("GetFieldDefinition")
                .WithDisplayName("GetFieldDefinition")
                .WithSummary("Gets a field definition from a class of a project by its name")
                .WithDescription("Gets a field definitionfrom a class of a project by its name")
                .Produces<FieldDefinitionModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldDefinitionsApi.MapPost("/{wipProjectId}/{className}",
                async (IFieldDefinitionService fieldDefinitionService,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Name of class")] string className,
                [Description("Field creation requests")][FromBody] List<FieldDefinitionModel> createRequests)
                    => await CreateFieldDefinitions.Handle(fieldDefinitionService, wipProjectId, className, createRequests))
                .WithName("CreateFieldDefinitions")
                .WithDisplayName("CreateFieldDefinitions")
                .WithSummary("Creates field definitions in a class of a WIP project")
                .WithDescription("Creates field definitions in a class of a WIP project")
                .Produces<List<FieldDefinitionModel>>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldDefinitionsApi.MapPut("/{wipProjectId}/{className}/{fieldName}",
                async (IFieldDefinitionService fieldDefinitionService,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Name of class")] string className,
                [Description("Name of class")] string fieldName,
                [Description("Field update request")][FromBody] FieldDefinitionModel updateRequest)
                    => await UpdateFieldDefinition.Handle(fieldDefinitionService, wipProjectId, className, fieldName, updateRequest))
                .WithName("UpdateFieldDefinition")
                .WithDisplayName("UpdateFieldDefinition")
                .WithSummary("Updates field definition in a class of a WIP project")
                .WithDescription("Uodates field definition in a class of a WIP project")
                .Produces<List<FieldDefinitionModel>>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldDefinitionsApi.MapDelete("/{wipProjectId}/{className}/{fieldName}",
                async (IFieldDefinitionService fieldDefinitionService,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Name of class")] string className,
                [Description("Name of field")] string fieldName)
                    => await DeleteFieldDefinition.Handle(fieldDefinitionService, wipProjectId, className, fieldName))
                .WithName("DeleteFieldDefinition")
                .WithDisplayName("DeleteFieldDefinition")
                .WithSummary("Deletes a field definition from a class of a WIP project by its name")
                .WithDescription("Deletes a field definition from a class of a WIP project by its name")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            return app;
        }
    }
}
