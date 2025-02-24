
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.HistoryRepositories;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.HistoryRepositories;

namespace pva.SuperV.Api.Routes.HistoryRepositories
{
    internal static class GetHistoryRepository
    {
        internal static Results<Ok<HistoryRepositoryModel>, NotFound<string>, BadRequest<string>, InternalServerError<string>> Handle(IHistoryRepositoryService historyRepositoryService, string projectId, string historyRepositoryName)
        {
            try
            {
                return TypedResults.Ok(historyRepositoryService.GetHistoryRepository(projectId, historyRepositoryName));
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