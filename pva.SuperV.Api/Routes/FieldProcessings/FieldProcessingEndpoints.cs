using Microsoft.AspNetCore.Mvc;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;
using System.ComponentModel;

namespace pva.SuperV.Api.Routes.FieldProcessings
{
    public static class FieldProcessingEndpoints
    {
        public static WebApplication MapFieldProcessingEndpoints(this WebApplication app)
        {
            RouteGroupBuilder fieldProcessingsApi = app.MapGroup("/field-processings");
            fieldProcessingsApi.MapGet("/{projectId}/{className}/{fieldName}",
                async (IFieldProcessingService fieldProcessingService,
                [Description("ID of project")] string projectId,
                [Description("Name of class")] string className,
                [Description("Name of field")] string fieldName)
                    => await GetProcessings.Handle(fieldProcessingService, projectId, className, fieldName))
                .WithName("GetFieldProcessings")
                .WithDisplayName("GetFieldProcessings")
                .WithSummary("Gets the list of available processings of a field in a class from a project")
                .WithDescription("Gets the list of available processings of a field in a class from a project")
                .Produces<List<FieldDefinitionModel>>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldProcessingsApi.MapGet("/{projectId}/{className}/{fieldName}/{processingName}",
                async (IFieldProcessingService fieldProcessingService,
                [Description("ID of project")] string projectId,
                [Description("Name of class")] string className,
                [Description("Name of field")] string fieldName,
                [Description("Name of processing")] string processingName)
                    => await GetProcessing.Handle(fieldProcessingService, projectId, className, fieldName, processingName))
                .WithName("GetFieldProcessing")
                .WithDisplayName("GetFieldProcessing")
                .WithSummary("Gets a field processing from a class of a project by its name")
                .WithDescription("Gets a field processing from a class of a project by its name")
                .Produces<FieldDefinitionModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldProcessingsApi.MapPut("/{wipProjectId}/{className}/{fieldName}/{processingName}",
                async (IFieldProcessingService fieldProcessingService,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Name of class")] string className,
                [Description("Name of field")] string fieldName,
                [Description("Name of field processing")] string processingName,
                [Description("Field processing update request")][FromBody] FieldValueProcessingModel createRequest)
                    => await UpdateProcessing.Handle(fieldProcessingService, wipProjectId, className, fieldName, processingName, createRequest))
                .WithName("UpdateProcessing")
                .WithDisplayName("UpdateProcessing")
                .WithSummary("Updates a field processing in a class of a WIP project")
                .WithDescription("Updates a field processing in a class of a WIP project")
                .Produces<FieldDefinitionModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldProcessingsApi.MapPost("/{wipProjectId}/{className}/{fieldName}",
                async (IFieldProcessingService fieldProcessingService,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Name of class")] string className,
                [Description("Name of field")] string fieldName,
                [Description("Field processing creation request")][FromBody] FieldValueProcessingModel createRequest)
                    => await CreateProcessing.Handle(fieldProcessingService, wipProjectId, className, fieldName, createRequest))
                .WithName("CreateProcessing")
                .WithDisplayName("CreateProcessing")
                .WithSummary("Creates a field processing in a class of a WIP project")
                .WithDescription("Creates a field processing in a class of a WIP project")
                .Produces<FieldDefinitionModel>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldProcessingsApi.MapDelete("/{wipProjectId}/{className}/{fieldName}/{processingName}",
                async (IFieldProcessingService fieldProcessingService,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Name of class")] string className,
                [Description("Name of field")] string fieldName,
                [Description("Name of processing")] string processingName)
                    => await DeleteProcessing.Handle(fieldProcessingService, wipProjectId, className, fieldName, processingName))
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
