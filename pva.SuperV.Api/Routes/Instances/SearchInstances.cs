using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.Instances;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Instances
{
    internal static class SearchInstances
    {
        internal static async Task<Results<Ok<PagedSearchResult<InstanceModel>>, NotFound<string>, BadRequest<string>>>
            Handle(IInstanceService instanceService, string projectId, InstancePagedSearchRequest search)
        {
            try
            {
                return TypedResults.Ok(await instanceService.SearchInstancesAsync(projectId, search));
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