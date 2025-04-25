
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.HistoryRepositories;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.HistoryRepositories
{
    internal static class GetHistoryRepositories
    {
        internal static async Task<Results<Ok<List<HistoryRepositoryModel>>, NotFound<string>, BadRequest<string>>>
            Handle(IHistoryRepositoryService historyRepositoryService, string projectId)
        {
            try
            {
                return TypedResults.Ok(await historyRepositoryService.GetHistoryRepositoriesAsync(projectId));
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