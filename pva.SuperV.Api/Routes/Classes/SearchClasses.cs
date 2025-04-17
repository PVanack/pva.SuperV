using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Classes;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.Classes;

namespace pva.SuperV.Api.Routes.Classes
{
    internal static class SearchClasses
    {
        internal static Results<Ok<PagedSearchResult<ClassModel>>, NotFound<string>, BadRequest<string>> Handle(IClassService classService, string projectId, ClassPagedSearchRequest search)
        {
            try
            {
                return TypedResults.Ok(classService.SearchClasses(projectId, search));
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