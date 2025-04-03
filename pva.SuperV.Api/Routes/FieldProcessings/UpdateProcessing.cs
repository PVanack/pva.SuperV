using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.FieldProcessings;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldProcessings;

namespace pva.SuperV.Api.Routes.FieldProcessings
{
    internal static class UpdateProcessing
    {
        internal static Results<Ok<FieldValueProcessingModel>, NotFound<string>, BadRequest<string>> Handle(IFieldProcessingService fieldProcessingService,
            string projectId, string className, string fieldName, string processingName, FieldValueProcessingModel createRequest)
        {
            try
            {
                FieldValueProcessingModel updatedFieldProcessing = fieldProcessingService.UpdateFieldProcessing(projectId, className, fieldName, processingName, createRequest);
                return TypedResults.Ok<FieldValueProcessingModel>(updatedFieldProcessing);
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