using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.FieldFormatters;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    internal static class GetFieldFormatterTypes
    {
        internal static Results<Ok<List<string>>, BadRequest<string>> Handle(IFieldFormatterService fieldFormatterService)
        {
            try
            {
                List<string> formatterTypes = fieldFormatterService.GetFieldFormatterTypes();
                return TypedResults.Ok<List<string>>(formatterTypes);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }

    }
}
