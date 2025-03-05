using pva.SuperV.Api.Services.HistoryRepositories;
using pva.SuperV.Model.HistoryRepositories;
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

            historyRepositoriesApi.MapPost("/{projectId}",
                (IHistoryRepositoryService historyRepositoryService,
                [Description("ID of project")] string projectId,
                [Description("History repository create request")] HistoryRepositoryModel historyRepositoryCreateRequest)
                    => CreateHistoryRepository.Handle(historyRepositoryService, projectId, historyRepositoryCreateRequest))
                .WithName("CreateHistoryRepository")
                .WithDisplayName("CreateHistoryRepository")
                .WithSummary("Creates a project's history repository")
                .WithDescription("Creates a project's history repository")
                .Produces<HistoryRepositoryModel>(StatusCodes.Status201Created)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            historyRepositoriesApi.MapDelete("/{projectId}/{historyRepositoryName}",
                (IHistoryRepositoryService historyRepositoryService,
                [Description("ID of project")] string projectId,
                [Description("Name of history repository")] string historyRepositoryName)
                    => DeleteHistoryRepository.Handle(historyRepositoryService, projectId, historyRepositoryName))
                .WithName("DeleteHistoryRepository")
                .WithDisplayName("DeleteHistoryRepository")
                .WithSummary("Deletes a project's history repository by its name")
                .WithDescription("Deletes a project's history repository by its name")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            return app;
        }
    }
}
