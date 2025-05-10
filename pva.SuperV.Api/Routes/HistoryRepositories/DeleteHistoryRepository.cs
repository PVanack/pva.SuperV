
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.HistoryRepositories
{
    internal static class DeleteHistoryRepository
    {
        internal static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> Handle(IHistoryRepositoryService historyRepositoryService, string projectId, string historyRepositoryName)
        {
            try
            {
                await historyRepositoryService.DeleteHistoryRepositoryAsync(projectId, historyRepositoryName);
                return TypedResults.NoContent();
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound<string>(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}