using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.FieldFormatters;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.FieldFormatters;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    internal static class SearchFieldFormatters
    {
        internal static Results<Ok<PagedSearchResult<FieldFormatterModel>>, NotFound<string>, BadRequest<string>> Handle(IFieldFormatterService fieldFormatterService,
            string projectId, FieldFormatterPagedSearchRequest search)
        {
            try
            {
                return TypedResults.Ok<PagedSearchResult<FieldFormatterModel>>(fieldFormatterService.SearchFieldFormatters(projectId, search));
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