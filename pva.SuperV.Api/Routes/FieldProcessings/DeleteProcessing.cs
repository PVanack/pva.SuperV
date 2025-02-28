using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.FieldProcessings;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Api.Routes.FieldProcessings
{
    internal static class DeleteProcessing
    {
        internal static Results<NoContent, NotFound<string>, BadRequest<string>, InternalServerError<string>> Handle(IFieldProcessingService fieldProcessingService, string projectId, string className, string fieldName, string processingName)
        {
            try
            {
                fieldProcessingService.DeleteFieldProcessing(projectId, className, fieldName, processingName);
                return TypedResults.NoContent();
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound<string>(e.Message);
            }
            catch (NonWipProjectException e)
            {
                return TypedResults.BadRequest<string>(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.InternalServerError(e.Message);
            }
        }
    }
}