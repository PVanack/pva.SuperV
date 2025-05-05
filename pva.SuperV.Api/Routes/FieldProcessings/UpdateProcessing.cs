using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.FieldProcessings
{
    internal static class UpdateProcessing
    {
        internal static async Task<Results<Ok<FieldValueProcessingModel>, NotFound<string>, BadRequest<string>>>
            Handle(IFieldProcessingService fieldProcessingService, string projectId, string className, string fieldName, string processingName, FieldValueProcessingModel updateRequest)
        {
            try
            {
                FieldValueProcessingModel updatedFieldProcessing = await fieldProcessingService.UpdateFieldProcessingAsync(projectId, className, fieldName, processingName, updateRequest);
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