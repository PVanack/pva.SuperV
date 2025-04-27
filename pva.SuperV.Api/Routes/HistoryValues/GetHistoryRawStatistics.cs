using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.HistoryRetrieval;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.HistoryValues
{
    internal static class GetHistoryRawStatistics
    {
        internal static async Task<Results<Ok<HistoryStatisticsRawResultModel>, NotFound<string>, BadRequest<string>>>
            Handle(IHistoryValuesService historyValuesService, string projectId, string instanceName, HistoryStatisticsRequestModel request)
        {
            try
            {
                HistoryStatisticsRawResultModel value = await historyValuesService.GetInstanceRawHistoryStatisticsAsync(projectId, instanceName, request);
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