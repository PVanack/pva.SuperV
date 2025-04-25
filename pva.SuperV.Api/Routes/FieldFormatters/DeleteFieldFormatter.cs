
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    internal static class DeleteFieldFormatter
    {
        internal static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>>
            Handle(IFieldFormatterService fieldFormatterService, string projectId, string fieldFormatterName)
        {
            try
            {
                await fieldFormatterService.DeleteFieldFormatterAsync(projectId, fieldFormatterName);
                return TypedResults.NoContent();
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound<string>(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest<string>(e.Message);
            }
        }
    }
}