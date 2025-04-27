using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.HistoryRepositories;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.HistoryRepositories
{
    internal static class CreateHistoryRepository
    {
        internal static async Task<Results<Created<HistoryRepositoryModel>, NotFound<string>, BadRequest<string>>>
            Handle(IHistoryRepositoryService historyRepositoryService, string projectId, HistoryRepositoryModel historyRepositoryCreateRequest)
        {
            try
            {
                HistoryRepositoryModel createdHistoryRepository = await historyRepositoryService.CreateHistoryRepositoryAsync(projectId, historyRepositoryCreateRequest);
                return TypedResults.Created<HistoryRepositoryModel>($"/history-repositories/{projectId}/{createdHistoryRepository.Name}", createdHistoryRepository);
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