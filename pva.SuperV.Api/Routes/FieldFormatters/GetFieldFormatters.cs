using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldFormatters;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    internal static class GetFieldFormatters
    {
        internal static async Task<Results<Ok<List<FieldFormatterModel>>, NotFound<string>, BadRequest<string>>>
            Handle(IFieldFormatterService fieldFormatterService, string projectId)
        {
            try
            {
                return TypedResults.Ok(await fieldFormatterService.GetFieldFormattersAsync(projectId));
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
