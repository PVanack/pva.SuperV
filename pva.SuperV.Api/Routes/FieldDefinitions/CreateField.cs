using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.FieldDefinitions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldDefinitions;

namespace pva.SuperV.Api.Routes.FieldDefinitions
{
    internal static class CreateField
    {
        internal static Results<Created<FieldDefinitionModel>, NotFound<string>, BadRequest<string>> Handle(IFieldDefinitionService fieldDefinitionService, string projectId, string className, FieldDefinitionModel createRequest)
        {
            try
            {
                FieldDefinitionModel createdField = fieldDefinitionService.CreateField(projectId, className, createRequest);
                return TypedResults.Created<FieldDefinitionModel>($"/fields/{projectId}/{className}/{createdField.Name}", createdField);
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