using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.FieldDefinitions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldDefinitions;

namespace pva.SuperV.Api.Routes.FieldDefinitions
{
    internal static class GetFieldDefinition
    {
        internal static Results<Ok<FieldDefinitionModel>, NotFound<string>, InternalServerError<string>> Handle(IFieldDefinitionService fieldDefinitionService, string projectId, string className, string fieldName)
        {
            try
            {
                FieldDefinitionModel fieldDefinitionModel = fieldDefinitionService.GetField(projectId, className, fieldName);
                return TypedResults.Ok<FieldDefinitionModel>(fieldDefinitionModel);
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.InternalServerError(e.Message);
            }
        }
    }
}