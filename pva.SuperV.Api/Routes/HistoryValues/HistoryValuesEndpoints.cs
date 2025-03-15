using Microsoft.AspNetCore.Mvc;
using pva.SuperV.Api.Services.History;
using pva.SuperV.Model.HistoryRetrieval;
using System.ComponentModel;

namespace pva.SuperV.Api.Routes.HistoryValues
{
    public static class HistoryValuesEndpoints
    {
        public static WebApplication MapHistoryValuesEndpoints(this WebApplication app)
        {
            RouteGroupBuilder historyValuesApi = app.MapGroup("/history-values");
            historyValuesApi.MapPost("/{projectId}/{instanceName}/raw",
                (IHistoryValuesService historyValuesService,
                [Description("ID of project")] string projectId,
                [Description("Name of instance")] string instanceName,
                [Description("History request")][FromBody] HistoryRequestModel request)
                    => GetHistoryRawValues.Handle(historyValuesService, projectId, instanceName, request))
                .WithName("GetInstanceRawValuesHistory")
                .WithDisplayName("GetInstanceRawValuesHistory")
                .WithSummary("Gets history raw values of instance fields between 2 dates.")
                .WithDescription("Gets history raw values of instance fields between 2 dates.")
                .Produces<HistoryRawResultModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);
            historyValuesApi.MapPost("/{projectId}/{instanceName}",
                (IHistoryValuesService historyValuesService,
                [Description("ID of project")] string projectId,
                [Description("Name of instance")] string instanceName,
                [Description("History request")][FromBody] HistoryRequestModel request)
                    => GetHistoryValues.Handle(historyValuesService, projectId, instanceName, request))
                .WithName("GetInstanceValuesHistory")
                .WithDisplayName("GetInstanceValuesHistory")
                .WithSummary("Gets history values of instance fields between 2 dates.")
                .WithDescription("Gets history values of instance fields between 2 dates.")
                .Produces<HistoryResultModel>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status400BadRequest);

            return app;
        }
    }
}
