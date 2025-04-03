using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.FieldDefinitions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldDefinitions;

namespace pva.SuperV.Api.Routes.FieldDefinitions
{
    internal static class UpdateFieldDefinition
    {
        internal static Results<Ok<FieldDefinitionModel>, NotFound<string>, BadRequest<string>> Handle(IFieldDefinitionService fieldDefinitionService, string projectId, string className, string fieldName, FieldDefinitionModel updateRequest)
        {
            try
            {
                FieldDefinitionModel updatedFieldDefinition = fieldDefinitionService.UpdateField(projectId, className, fieldName, updateRequest);
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