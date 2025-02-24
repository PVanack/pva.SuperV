
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Routes.Projects;
using pva.SuperV.Api.Services.HistoryRepositories;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.HistoryRepositories;

namespace pva.SuperV.Api.Routes.HistoryRepositories
{
    internal static class GetHistoryRepositories
    {
        internal static Results<Ok<List<HistoryRepositoryModel>>, NotFound<string>, BadRequest<string>, InternalServerError<string>> Handle(IHistoryRepositoryService historyRepositoryService, string projectId)
        {
            try
            {
                return TypedResults.Ok(historyRepositoryService.GetHistoryRepositories(projectId));
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