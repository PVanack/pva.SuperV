using pva.SuperV.Model.HistoryRepositories;
using pva.SuperV.Model.Services;
using System.ComponentModel;

namespace pva.SuperV.Api.Routes.HistoryRepositories
{
    public static class HistoryRepositoryEndpoints
    {
        public static WebApplication MapHistoryRepositoryEndpoints(this WebApplication app)
        {
            RouteGroupBuilder historyRepositoriesApi = app.MapGroup("/history-repositories");
            historyRepositoriesApi.MapGet("/{projectId}",
                (IHistoryRepositoryService historyRepositoryService,
                [Description("ID of project")] string projectId)
                    => GetHistoryRepositories.Handle(historyRepositoryService, projectId))
                .WithName("GetHistoryRepositories")
                .WithDisplayName("GetHistoryRepositories")
                .WithSummary("Gets the list of available history respoitories in project")
                .WithDescription("Gets the list of available history respoitories in project")
                .Produces<List<HistoryRepositoryModel>>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            historyRepositoriesApi.MapGet("/{projectId}/{historyRepositoryName}",
                (IHistoryRepositoryService historyRepositoryService,
                [Description("ID of project")] string projectId,
                [Description("Name of history repository")] string historyRepositoryName)
                    => GetHistoryRepository.Handle(historyRepositoryService, projectId, historyRepositoryName))
                .WithName("GetHistoryRepository")
                .WithDisplayName("GetHistoryRepository")
                .WithSummary("Gets a project's history repository by its name")
                .WithDescription("Gets a project's history repository by its name")
                .Produces<HistoryRepositoryModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            historyRepositoriesApi.MapPost("/{wipProjectId}",
                (IHistoryRepositoryService historyRepositoryService,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("History repository create request")] HistoryRepositoryModel historyRepositoryCreateRequest)
                    => CreateHistoryRepository.Handle(historyRepositoryService, wipProjectId, historyRepositoryCreateRequest))
                .WithName("CreateHistoryRepository")
                .WithDisplayName("CreateHistoryRepository")
                .WithSummary("Creates a history repository in a WIP project")
                .WithDescription("Creates a history repository in a WIP project")
                .Produces<HistoryRepositoryModel>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            historyRepositoriesApi.MapPut("/{wipProjectId}/{historyRepositoryName}",
                (IHistoryRepositoryService historyRepositoryService,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Name of history repository")] string historyRepositoryName,
                [Description("History repository update request")] HistoryRepositoryModel historyRepositoryUpdateRequest)
                    => UpdateHistoryRepository.Handle(historyRepositoryService, wipProjectId, historyRepositoryName, historyRepositoryUpdateRequest))
                .WithName("UpdateHistoryRepository")
                .WithDisplayName("UpdateHistoryRepository")
                .WithSummary("Updates a history repository in a WIP project")
                .WithDescription("Updates a history repository in a WIP project")
                .Produces<HistoryRepositoryModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            historyRepositoriesApi.MapDelete("/{wipProjectId}/{historyRepositoryName}",
                (IHistoryRepositoryService historyRepositoryService,
                [Description("ID of WIP project")] string wipProjectId,
                [Description("Name of history repository")] string historyRepositoryName)
                    => DeleteHistoryRepository.Handle(historyRepositoryService, wipProjectId, historyRepositoryName))
                .WithName("DeleteHistoryRepository")
                .WithDisplayName("DeleteHistoryRepository")
                .WithSummary("Deletes a history repository from a WIP project by its name")
                .WithDescription("Deletes a history repository from a WIP project by its name")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            return app;
        }
    }
}
