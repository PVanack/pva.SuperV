
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.HistoryRepositories;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Api.Routes.HistoryRepositories
{
    internal static class DeleteHistoryRepository
    {
        internal static Results<NoContent, NotFound<string>, BadRequest<string>, InternalServerError<string>> Handle(IHistoryRepositoryService historyRepositoryService, string projectId, string historyRepositoryName)
        {
            try
            {
                historyRepositoryService.DeleteHistoryRepository(projectId, historyRepositoryName);
                return TypedResults.NoContent();
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound<string>(e.Message);
            }
            catch (NonWipProjectException e)
            {
                return TypedResults.BadRequest<string>(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.InternalServerError(e.Message);
            }
        }
    }
}