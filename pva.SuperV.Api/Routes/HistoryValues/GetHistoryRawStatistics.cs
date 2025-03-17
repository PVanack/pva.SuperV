using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.History;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.HistoryRetrieval;

namespace pva.SuperV.Api.Routes.HistoryValues
{
    internal class GetHistoryRawStatistics
    {
        internal static Results<Ok<HistoryStatisticsRawResultModel>, NotFound<string>, BadRequest<string>> Handle(IHistoryValuesService historyValuesService, string projectId, string instanceName, HistoryStatisticsRequestModel request)
        {
            try
            {
                if (request.StartTime >= request.EndTime)
                {
                    return TypedResults.BadRequest("Start time needs to be before end time");
                }
                HistoryStatisticsRawResultModel value = historyValuesService.GetInstanceRawHistoryStatistics(projectId, instanceName, request);
                return TypedResults.Ok(value);
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}