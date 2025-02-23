using pva.SuperV.Api.Routes.Projects;
using pva.SuperV.Model.HistoryRepositories;
using pva.SuperV.Model.Projects;
using System.ComponentModel;

namespace pva.SuperV.Api.Routes.HistoryRepositories
{
    public static class HistoryRepositoryEndpoints
    {
        public static WebApplication MapHistoryRepositoryEndpoints(this WebApplication app)
        {
            RouteGroupBuilder projectsApi = app.MapGroup("/history-repositories");
            projectsApi.MapGet("/{projectId}",
                (IHistoryRepositoryService historyRepositoryService,
                [Description("ID of project")] string projectId)
                    => GetHistoryRepositories.Handle(historyRepositoryService, projectId))
                .WithName("GetHistoryRepositories")
                .WithDisplayName("GetHistoryRepositories")
                .WithSummary("Gets the list of available history respoitories in project")
                .WithDescription("Gets the list of available history respoitories in project")
                .Produces<List<HistoryRepositoryModel>>(StatusCodes.Status200OK);

            projectsApi.MapGet("/{projectId}/{historyRepositoryName}",
                (IHistoryRepositoryService historyRepositoryService,
                [Description("ID of project")] string projectId,
                [Description("Name of history repository")] string historyRepositoryName)
                    => GetHistoryRepository.Handle(historyRepositoryService, projectId, historyRepositoryName))
                .WithName("GetHistoryRepository")
                .WithDisplayName("GetHistoryRepository")
                .WithSummary("Gets a project's history repository by its name")
                .WithDescription("Gets a project's history repository by its name")
                .Produces<HistoryRepositoryModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound);
            return app;
        }
    }
}
