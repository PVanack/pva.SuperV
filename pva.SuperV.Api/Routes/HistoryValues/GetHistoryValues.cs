using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.HistoryRetrieval;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.HistoryValues
{
    internal static class GetHistoryValues
    {
        internal static async Task<Results<Ok<HistoryResultModel>, NotFound<string>, BadRequest<string>>>
            Handle(IHistoryValuesService historyValuesService, string projectId, string instanceName, HistoryRequestModel request)
        {
            try
            {
                if (request.StartTime >= request.EndTime)
                {
                    return TypedResults.BadRequest("Start time needs to be before end time");
                }
                HistoryResultModel value = await historyValuesService.GetInstanceHistoryValuesAsync(projectId, instanceName, request);
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