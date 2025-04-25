using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    internal static class GetFieldFormatterTypes
    {
        internal static async Task<Results<Ok<List<string>>, BadRequest<string>>>
            Handle(IFieldFormatterService fieldFormatterService)
        {
            try
            {
                List<string> formatterTypes = await fieldFormatterService.GetFieldFormatterTypesAsync();
                return TypedResults.Ok<List<string>>(formatterTypes);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }

    }
}
