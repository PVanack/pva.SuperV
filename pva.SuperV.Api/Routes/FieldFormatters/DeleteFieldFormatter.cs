
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    internal static class DeleteFieldFormatter
    {
        internal static Results<Ok, NotFound<string>, BadRequest<string>, InternalServerError<string>> Handle(IFieldFormatterService fieldFormatterService, string projectId, string fieldFormatterName)
        {
            try
            {
                fieldFormatterService.DeleteFieldFormatter(projectId, fieldFormatterName);
                return TypedResults.Ok();
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