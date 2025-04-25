using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.FieldFormatters;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    internal static class SearchFieldFormatters
    {
        internal static async Task<Results<Ok<PagedSearchResult<FieldFormatterModel>>, NotFound<string>, BadRequest<string>>>
            Handle(IFieldFormatterService fieldFormatterService, string projectId, FieldFormatterPagedSearchRequest search)
        {
            try
            {
                return TypedResults.Ok<PagedSearchResult<FieldFormatterModel>>(await fieldFormatterService.SearchFieldFormattersAsync(projectId, search));
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