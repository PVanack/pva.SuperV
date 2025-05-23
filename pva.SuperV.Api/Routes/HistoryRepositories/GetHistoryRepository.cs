﻿
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.HistoryRepositories;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.HistoryRepositories
{
    internal static class GetHistoryRepository
    {
        internal static async Task<Results<Ok<HistoryRepositoryModel>, NotFound<string>, BadRequest<string>>>
            Handle(IHistoryRepositoryService historyRepositoryService, string projectId, string historyRepositoryName)
        {
            try
            {
                return TypedResults.Ok(await historyRepositoryService.GetHistoryRepositoryAsync(projectId, historyRepositoryName));
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