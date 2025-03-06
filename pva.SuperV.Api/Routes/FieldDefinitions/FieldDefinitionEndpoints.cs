using Microsoft.AspNetCore.Mvc;
using pva.SuperV.Api.Services.FieldDefinitions;
using pva.SuperV.Model.FieldDefinitions;
using System.ComponentModel;

namespace pva.SuperV.Api.Routes.FieldDefinitions
{
    public static class FieldDefinitionEndpoints
    {
        public static WebApplication MapFieldDefinitionEndpoints(this WebApplication app)
        {
            RouteGroupBuilder fieldDefinitionsApi = app.MapGroup("/fields");
            fieldDefinitionsApi.MapGet("/{projectId}/{className}",
                (IFieldDefinitionService fieldDefinitionService,
                [Description("ID of project")] string projectId,
                [Description("Name of class")] string className)
                    => GetFieldDefinitions.Handle(fieldDefinitionService, projectId, className))
                .WithName("GetFieldDefinitions")
                .WithDisplayName("GetFieldDefinitions")
                .WithSummary("Gets the list of available fields in a class from a project")
                .WithDescription("Gets the list of available fields in a class from a project")
                .Produces<List<FieldDefinitionModel>>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldDefinitionsApi.MapGet("/{projectId}/{className}/{fieldName}",
                (IFieldDefinitionService fieldDefinitionService,
                [Description("ID of project")] string projectId,
                [Description("Name of class")] string className,
                [Description("Name of field")] string fieldName)
                    => GetFieldDefinition.Handle(fieldDefinitionService, projectId, className, fieldName))
                .WithName("GetFieldDefinition")
                .WithDisplayName("GetFieldDefinition")
                .WithSummary("Gets a field definition from a class of a project by its name")
                .WithDescription("Gets a field definitionfrom a class of a project by its name")
                .Produces<FieldDefinitionModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldDefinitionsApi.MapPost("/{projectId}/{className}",
                (IFieldDefinitionService fieldDefinitionService,
                [Description("ID of project")] string projectId,
                [Description("Name of class")] string className,
                [Description("Field creation requests")][FromBody] List<FieldDefinitionModel> createRequests)
                    => CreateField.Handle(fieldDefinitionService, projectId, className, createRequests))
                .WithName("CreateFieldDefinitions")
                .WithDisplayName("CreateFieldDefinitions")
                .WithSummary("Creates field definitions in a class of a project")
                .WithDescription("Creates field definitions in a class of a project")
                .Produces(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldDefinitionsApi.MapDelete("/{projectId}/{className}/{fieldName}",
                (IFieldDefinitionService fieldDefinitionService,
                [Description("ID of project")] string projectId,
                [Description("Name of class")] string className,
                [Description("Name of field")] string fieldName)
                    => DeleteField.Handle(fieldDefinitionService, projectId, className, fieldName))
                .WithName("DeleteFieldDefinition")
                .WithDisplayName("DeleteFieldDefinition")
                .WithSummary("Deletes a field definition from a class of a project by its name")
                .WithDescription("Deletes a field definition from a class of a project by its name")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            return app;
        }
    }
}
