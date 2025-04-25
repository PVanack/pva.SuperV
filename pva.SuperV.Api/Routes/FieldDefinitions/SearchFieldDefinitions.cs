using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.FieldDefinitions
{
    internal static class SearchFieldDefinitions
    {
        internal static async Task<Results<Ok<PagedSearchResult<FieldDefinitionModel>>, NotFound<string>, BadRequest<string>>>
            Handle(IFieldDefinitionService fieldDefinitionService, string projectId, string className, FieldDefinitionPagedSearchRequest search)
        {
            try
            {
                return TypedResults.Ok(await fieldDefinitionService.SearchFieldsAsync(projectId, className, search));
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