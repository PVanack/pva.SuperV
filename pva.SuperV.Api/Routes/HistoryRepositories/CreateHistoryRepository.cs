using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.HistoryRepositories;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.HistoryRepositories;

namespace pva.SuperV.Api.Routes.HistoryRepositories
{
    internal static class CreateHistoryRepository
    {
        internal static Results<Created<HistoryRepositoryModel>, NotFound<string>, BadRequest<string>, InternalServerError<string>> Handle(IHistoryRepositoryService historyRepositoryService, string projectId, HistoryRepositoryModel historyRepositoryCreateRequest)
        {
            try
            {
                HistoryRepositoryModel createdHistoryRepository = historyRepositoryService.CreateHistoryRepository(projectId, historyRepositoryCreateRequest);
                return TypedResults.Created<HistoryRepositoryModel>($"/history-repositories/{projectId}/{createdHistoryRepository.Name}", createdHistoryRepository);
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.InternalServerError(e.Message);
            }
        }
    }
}