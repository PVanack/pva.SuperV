using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Classes
{
    internal static class SearchClasses
    {
        internal static async Task<Results<Ok<PagedSearchResult<ClassModel>>, NotFound<string>, BadRequest<string>>>
            Handle(IClassService classService, string projectId, ClassPagedSearchRequest search)
        {
            try
            {
                return TypedResults.Ok(await classService.SearchClassesAsync(projectId, search));
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