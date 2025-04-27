using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.FieldDefinitions
{
    internal static class UpdateFieldDefinition
    {
        internal static async Task<Results<Ok<FieldDefinitionModel>, NotFound<string>, BadRequest<string>>>
            Handle(IFieldDefinitionService fieldDefinitionService, string projectId, string className, string fieldName, FieldDefinitionModel updateRequest)
        {
            try
            {
                FieldDefinitionModel updatedFieldDefinition = await fieldDefinitionService.UpdateFieldAsync(projectId, className, fieldName, updateRequest);
                return TypedResults.Ok(updatedFieldDefinition);
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