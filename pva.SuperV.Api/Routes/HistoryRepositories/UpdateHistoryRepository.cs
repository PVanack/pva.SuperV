using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.HistoryRepositories;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.HistoryRepositories;

namespace pva.SuperV.Api.Routes.HistoryRepositories
{
    internal static class UpdateHistoryRepository
    {
        internal static Results<Ok<HistoryRepositoryModel>, NotFound<string>, BadRequest<string>> Handle(IHistoryRepositoryService historyRepositoryService,
            string wipProjectId, string historyRepositoryName, HistoryRepositoryModel historyRepositoryUpdateRequest)
        {
            try
            {
                HistoryRepositoryModel updatedHistoryRepository = historyRepositoryService.UpdateHistoryRepository(wipProjectId, historyRepositoryName, historyRepositoryUpdateRequest);
                return TypedResults.Ok<HistoryRepositoryModel>(updatedHistoryRepository);
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