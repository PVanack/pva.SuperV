using Microsoft.AspNetCore.Mvc;
using pva.SuperV.Api.Services.Instances;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.Instances;
using System.ComponentModel;

namespace pva.SuperV.Api.Routes.Instances
{
    public static class InstancesEndpoints
    {
        public static WebApplication MapInstancesEndpoints(this WebApplication app)
        {
            RouteGroupBuilder fieldDefinitionsApi = app.MapGroup("/instances");
            fieldDefinitionsApi.MapGet("/{projectId}",
                (IInstanceService instanceService,
                [Description("ID of project")] string projectId)
                    => GetInstances.Handle(instanceService, projectId))
                .WithName("GetInstances")
                .WithDisplayName("GetInstances")
                .WithSummary("Gets the list of instances from a project")
                .WithDescription("Gets the list of instances from a project")
                .Produces<List<FieldDefinitionModel>>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldDefinitionsApi.MapGet("/{projectId}/{instanceName}",
                (IInstanceService instanceService,
                [Description("ID of project")] string projectId,
                [Description("Name of instance")] string instanceName)
                    => GetInstance.Handle(instanceService, projectId, instanceName))
                .WithName("GetInstance")
                .WithDisplayName("GetInstance")
                .WithSummary("Gets an instance of a project by its name")
                .WithDescription("Gets an instance of a project by its name")
                .Produces<FieldDefinitionModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldDefinitionsApi.MapPost("/{projectId}",
                (IInstanceService instanceService,
                [Description("ID of project")] string projectId,
                [Description("Instance creation request")][FromBody] InstanceModel createRequest)
                    => CreateInstance.Handle(instanceService, projectId, createRequest))
                .WithName("CreateInstance")
                .WithDisplayName("CreateInstance")
                .WithSummary("Creates an instance with a class of a project")
                .WithDescription("Creates an instance with a class of a project")
                .Produces<FieldDefinitionModel>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldDefinitionsApi.MapDelete("/{projectId}/{instanceName}",
                (IInstanceService instanceService,
                [Description("ID of project")] string projectId,
                [Description("Name of instance")] string instanceName)
                    => DeleteInstance.Handle(instanceService, projectId, instanceName))
                .WithName("DeleteInstance")
                .WithDisplayName("DeleteInstance")
                .WithSummary("Deletes an instance from a project by its name")
                .WithDescription("Deletes an instance from a project by its name")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldDefinitionsApi.MapGet("/{projectId}/{instanceName}/{fieldName}/value",
                (IFieldValueService fieldValueService,
                [Description("ID of project")] string projectId,
                [Description("Name of instance")] string instanceName,
                [Description("Name of field")] string fieldName)
                    => GetInstanceField.Handle(fieldValueService, projectId, instanceName, fieldName))
                .WithName("GetField")
                .WithDisplayName("GetField")
                .WithSummary("Gets the field of an instance from a project by its name")
                .WithDescription("Gets the field of an instance from a project by its name")
                .Produces(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            fieldDefinitionsApi.MapPut("/{projectId}/{instanceName}/{fieldName}/value",
                (IFieldValueService fieldValueService,
                [Description("ID of project")] string projectId,
                [Description("Name of instance")] string instanceName,
                [Description("Name of field")] string fieldName,
                [Description("Value of field")][FromBody] FieldValueModel value)
                    => UpdateInstanceFieldValue.Handle(fieldValueService, projectId, instanceName, fieldName, value))
                .WithName("UpdateFieldValue")
                .WithDisplayName("UpdateFieldValue")
                .WithSummary("Updates the value of a field of an instance from a project by its name")
                .WithDescription("Updates the value of a field of an instance from a project by its name")
                .Produces(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            return app;
        }
    }
}
