using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.FieldProcessings
{
    internal static class GetProcessing
    {
        internal static async Task<Results<Ok<FieldValueProcessingModel>, NotFound<string>, BadRequest<string>>>
            Handle(IFieldProcessingService fieldProcessingService, string projectId, string className, string fieldName, string processingName)
        {
            try
            {
                return TypedResults.Ok(await fieldProcessingService.GetFieldProcessingAsync(projectId, className, fieldName, processingName));
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