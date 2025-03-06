using Microsoft.AspNetCore.Mvc;
using pva.SuperV.Api.Services.FieldProcessings;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.FieldProcessings;
using System.ComponentModel;

namespace pva.SuperV.Api.Routes.FieldProcessings
{
    public static class FieldProcessingEndpoints
    {
        public static WebApplication MapFieldProcessingEndpoints(this WebApplication app)
        {
            RouteGroupBuilder fieldDefinitionsApi = app.MapGroup("/field-processings");
            fieldDefinitionsApi.MapGet("/{projectId}/{className}/{fieldName}",
                (IFieldProcessingService fieldProcessingService,
                [Description("ID of project")] string projectId,
                [Description("Name of class")] string className,
                [Description("Name of field")] string fieldName)
                    => GetProcessings.Handle(fieldProcessingService, projectId, className, fieldName))
                .WithName("GetFieldProcessings")
                .WithDisplayName("GetFieldProcessings")
                .WithSummary("Gets the list of available processings of a field in a class from a project")
                .WithDescription("Gets the list of available processings of a field in a class from a project")
                .Produces<List<FieldDefinitionModel>>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldDefinitionsApi.MapGet("/{projectId}/{className}/{fieldName}/{processingName}",
                (IFieldProcessingService fieldProcessingService,
                [Description("ID of project")] string projectId,
                [Description("Name of class")] string className,
                [Description("Name of field")] string fieldName,
                [Description("Name of processing")] string processingName)
                    => GetProcessing.Handle(fieldProcessingService, projectId, className, fieldName, processingName))
                .WithName("GetFieldProcessing")
                .WithDisplayName("GetFieldProcessing")
                .WithSummary("Gets a field processing from a class of a project by its name")
                .WithDescription("Gets a field processing from a class of a project by its name")
                .Produces<FieldDefinitionModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldDefinitionsApi.MapPost("/{wipProjectId}/{className}/{fieldName}",
                (IFieldProcessingService fieldProcessingService,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Name of class")] string className,
                [Description("Name of field")] string fieldName,
                [Description("Field processing creation request")][FromBody] FieldValueProcessingModel createRequest)
                    => CreateProcessing.Handle(fieldProcessingService, wipProjectId, className, fieldName, createRequest))
                .WithName("CreateProcessing")
                .WithDisplayName("CreateProcessing")
                .WithSummary("Creates a field processing in a class of a WIP project")
                .WithDescription("Creates a field processing in a class of a WIP project")
                .Produces<FieldDefinitionModel>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldDefinitionsApi.MapDelete("/{wipProjectId}/{className}/{fieldName}/{processingName}",
                (IFieldProcessingService fieldProcessingService,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Name of class")] string className,
                [Description("Name of field")] string fieldName,
                [Description("Name of processing")] string processingName)
                    => DeleteProcessing.Handle(fieldProcessingService, wipProjectId, className, fieldName, processingName))
                .WithName("DeleteProcessing")
                .WithDisplayName("DeleteProcessing")
                .WithSummary("Deletes a field processing from a class of a WIP project by its name")
                .WithDescription("Deletes a field processing from a class of a WIP project by its name")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            return app;
        }
    }
}
