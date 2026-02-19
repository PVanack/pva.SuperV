using Microsoft.AspNetCore.Mvc;
using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;
using System.ComponentModel;

namespace pva.SuperV.Api.Routes.Scripts
{
    public static class ScriptEndpoints
    {
        public static WebApplication MapScriptEndpoints(this WebApplication app)
        {
            RouteGroupBuilder scriptsApi = app.MapGroup("/scripts");
            scriptsApi.MapGet("/{projectId}",
                async (IScriptService scriptService,
                [Description("ID of project")] string projectId)
                    => await GetScripts.Handle(scriptService, projectId))
                .WithName("GetScripts")
                .WithDisplayName("GetScripts")
                .WithSummary("Gets the list of available scripts from a project")
                .WithDescription("Gets the list of scripts from a project")
                .Produces<List<ScriptDefinitionModel>>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            scriptsApi.MapGet("/{projectId}/{scriptName}",
                async (IScriptService scriptService,
                [Description("ID of project")] string projectId,
                [Description("Name of processing")] string scriptName)
                    => await GetScript.Handle(scriptService, projectId, scriptName))
                .WithName("GetScript")
                .WithDisplayName("GetScript")
                .WithSummary("Gets a field processing from a class of a project by its name")
                .WithDescription("Gets a field processing from a class of a project by its name")
                .Produces<ScriptDefinitionModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            scriptsApi.MapPut("/{wipProjectId}/{scriptName}",
                async (IScriptService scriptService,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Name of field processing")] string scriptName,
                [Description("Field processing update request")][FromBody] ScriptDefinitionModel createRequest)
                    => await UpdateScript.Handle(scriptService, wipProjectId, scriptName, createRequest))
                .WithName("UpdateScript")
                .WithDisplayName("UpdateScript")
                .WithSummary("Updates a field processing in a class of a WIP project")
                .WithDescription("Updates a field processing in a class of a WIP project")
                .Produces<ScriptDefinitionModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            scriptsApi.MapPost("/{wipProjectId}",
                async (IScriptService scriptService,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Script creation request")][FromBody] ScriptDefinitionModel createRequest)
                    => await CreateScript.Handle(scriptService, wipProjectId, createRequest))
                .WithName("CreateScript")
                .WithDisplayName("CreateScript")
                .WithSummary("Creates a script in a WIP project")
                .WithDescription("Creates a script in a WIP project")
                .Produces<ScriptDefinitionModel>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            scriptsApi.MapDelete("/{wipProjectId}/{scriptName}",
                async (IScriptService scriptService,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Name of script")] string scriptName)
                    => await DeleteScript.Handle(scriptService, wipProjectId, scriptName))
                .WithName("DeleteScript")
                .WithDisplayName("DeleteScript")
                .WithSummary("Deletes a script from a WIP project by its name")
                .WithDescription("Deletes a script from a WIP project by its name")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            return app;
        }
    }
}
