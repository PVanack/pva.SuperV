using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.FieldDefinitions
{
    internal static class GetFieldDefinition
    {
        internal static async Task<Results<Ok<FieldDefinitionModel>, NotFound<string>, BadRequest<string>>>
            Handle(IFieldDefinitionService fieldDefinitionService, string projectId, string className, string fieldName)
        {
            try
            {
                FieldDefinitionModel fieldDefinitionModel = await fieldDefinitionService.GetFieldAsync(projectId, className, fieldName);
                return TypedResults.Ok<FieldDefinitionModel>(fieldDefinitionModel);
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