using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    public static class GetFieldFormatters
    {
        internal static Results<Ok<List<FieldFormatterModel>>, InternalServerError<string>> Handle(IFieldFormatterService fieldFormatterService, string projectId)
        {
            try
            {
                List<FieldFormatterModel> fieldFormatters = fieldFormatterService.GetFieldFormatters(projectId);
                return TypedResults.Ok<List<FieldFormatterModel>>(fieldFormatters);
            }
            catch (SuperVException e)
            {
                return TypedResults.InternalServerError<string>(e.Message);
            }
        }

    }
}
