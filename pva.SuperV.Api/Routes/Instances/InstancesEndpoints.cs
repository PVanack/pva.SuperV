using Microsoft.AspNetCore.Mvc;
using pva.SuperV.Model;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.Instances;
using pva.SuperV.Model.Services;
using System.ComponentModel;

namespace pva.SuperV.Api.Routes.Instances
{
    public static class InstancesEndpoints
    {
        public static WebApplication MapInstancesEndpoints(this WebApplication app)
        {
            RouteGroupBuilder instancesApi = app.MapGroup("/instances");
            instancesApi.MapGet("/{runnableProjectId}",
                (IInstanceService instanceService,
                [Description("ID of runnable project")] string runnableProjectId)
                    => GetInstances.Handle(instanceService, runnableProjectId))
                .WithName("GetInstances")
                .WithDisplayName("GetInstances")
                .WithSummary("Gets the list of instances from a project")
                .WithDescription("Gets the list of instances from a project")
                .Produces<List<FieldDefinitionModel>>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            instancesApi.MapPost("/{runnableProjectId}/search",
                (IInstanceService instanceService,
                [Description("ID of runnable project")] string runnableProjectId,
                [FromBody] InstancePagedSearchRequest search)
                    => SearchInstances.Handle(instanceService, runnableProjectId, search))
                .WithName("SearchInstances")
                .WithDisplayName("SearchInstances")
                .WithSummary("Searches the list of instances from a project")
                .WithDescription("Searches the list of instances from a project")
                .Produces<PagedSearchResult<FieldDefinitionModel>>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            instancesApi.MapGet("/{runnableProjectId}/{instanceName}",
                (IInstanceService instanceService,
                [Description("ID of runnable project")] string runnableProjectId,
                [Description("Name of instance")] string instanceName)
                    => GetInstance.Handle(instanceService, runnableProjectId, instanceName))
                .WithName("GetInstance")
                .WithDisplayName("GetInstance")
                .WithSummary("Gets an instance of a project by its name")
                .WithDescription("Gets an instance of a project by its name")
                .Produces<FieldDefinitionModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            instancesApi.MapPost("/{runnableProjectId}",
                (IInstanceService instanceService,
                [Description("ID of runnable project")] string runnableProjectId,
                [Description("Instance creation request")][FromBody] InstanceModel createRequest)
                    => CreateInstance.Handle(instanceService, runnableProjectId, createRequest))
                .WithName("CreateInstance")
                .WithDisplayName("CreateInstance")
                .WithSummary("Creates an instance with a class of a project")
                .WithDescription("Creates an instance with a class of a project")
                .Produces<FieldDefinitionModel>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            instancesApi.MapDelete("/{runnableProjectId}/{instanceName}",
                (IInstanceService instanceService,
                [Description("ID of runnable project")] string runnableProjectId,
                [Description("Name of instance")] string instanceName)
                    => DeleteInstance.Handle(instanceService, runnableProjectId, instanceName))
                .WithName("DeleteInstance")
                .WithDisplayName("DeleteInstance")
                .WithSummary("Deletes an instance from a project by its name")
                .WithDescription("Deletes an instance from a project by its name")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            instancesApi.MapGet("/{runnableProjectId}/{instanceName}/{fieldName}/value",
                async (IFieldValueService fieldValueService,
                [Description("ID of runnable project")] string runnableProjectId,
                [Description("Name of instance")] string instanceName,
                [Description("Name of field")] string fieldName)
                    => await GetInstanceField.Handle(fieldValueService, runnableProjectId, instanceName, fieldName))
                .WithName("GetField")
                .WithDisplayName("GetField")
                .WithSummary("Gets the field of an instance from a project by its name")
                .WithDescription("Gets the field of an instance from a project by its name")
                .Produces<FieldModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            instancesApi.MapPut("/{runnableProjectId}/{instanceName}/{fieldName}/value",
                async (IFieldValueService fieldValueService,
                [Description("ID of runnable project")] string runnableProjectId,
                [Description("Name of instance")] string instanceName,
                [Description("Name of field")] string fieldName,
                [Description("Value of field")][FromBody] FieldValueModel value)
                    => await UpdateInstanceFieldValue.Handle(fieldValueService, runnableProjectId, instanceName, fieldName, value))
                .WithName("UpdateFieldValue")
                .WithDisplayName("UpdateFieldValue")
                .WithSummary("Updates the value of a field of an instance from a project by its name")
                .WithDescription("Updates the value of a field of an instance from a project by its name")
                .Produces<FieldValueModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            return app;
        }
    }
}
