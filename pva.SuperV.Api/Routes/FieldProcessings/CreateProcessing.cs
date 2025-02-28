using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.FieldProcessings;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldProcessings;

namespace pva.SuperV.Api.Routes.FieldProcessings
{
    internal static class CreateProcessing
    {
        internal static Results<Created<FieldValueProcessingModel>, BadRequest<string>> Handle(IFieldProcessingService fieldProcessingService, string projectId, string className, string fieldName, FieldValueProcessingModel createRequest)
        {
            try
            {
                FieldValueProcessingModel createdFieldProcessing = fieldProcessingService.CreateFieldProcessing(projectId, className, fieldName, createRequest);
                return TypedResults.Created<FieldValueProcessingModel>($"//field-processings/{projectId}/{className}/{fieldName}{createdFieldProcessing.Name}", createdFieldProcessing);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}