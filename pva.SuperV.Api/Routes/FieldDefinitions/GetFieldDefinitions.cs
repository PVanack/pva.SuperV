using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.FieldDefinitions
{
    internal static class GetFieldDefinitions
    {
        internal static async Task<Results<Ok<List<FieldDefinitionModel>>, NotFound<string>, BadRequest<string>>>
            Handle(IFieldDefinitionService fieldDefinitionService, string projectId, string className)
        {
            try
            {
                return TypedResults.Ok(await fieldDefinitionService.GetFieldsAsync(projectId, className));
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