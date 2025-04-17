using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.FieldDefinitions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.FieldDefinitions;

namespace pva.SuperV.Api.Routes.FieldDefinitions
{
    internal static class SearchFieldDefinitions
    {
        internal static Results<Ok<PagedSearchResult<FieldDefinitionModel>>, NotFound<string>, BadRequest<string>> Handle(IFieldDefinitionService fieldDefinitionService, string projectId, string className, FieldDefinitionPagedSearchRequest search)
        {
            try
            {
                return TypedResults.Ok(fieldDefinitionService.SearchFields(projectId, className, search));
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