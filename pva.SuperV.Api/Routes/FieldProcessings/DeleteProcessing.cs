using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.FieldProcessings
{
    internal static class DeleteProcessing
    {
        internal static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>>
            Handle(IFieldProcessingService fieldProcessingService, string projectId, string className, string fieldName, string processingName)
        {
            try
            {
                await fieldProcessingService.DeleteFieldProcessingAsync(projectId, className, fieldName, processingName);
                return TypedResults.NoContent();
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound<string>(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}