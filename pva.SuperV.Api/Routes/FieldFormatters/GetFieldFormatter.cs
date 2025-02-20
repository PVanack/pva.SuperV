using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldFormatters;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    public static class GetFieldFormatter
    {
        internal static Results<Ok<FieldFormatterModel>, InternalServerError<string>> Handle(IFieldFormatterService fieldFormatterService, string projectId, string fieldFormatterName)
        {
            try
            {
                FieldFormatterModel fieldFormatter = fieldFormatterService.GetFieldFormatter(projectId, fieldFormatterName);
                return TypedResults.Ok<FieldFormatterModel>(fieldFormatter);
            }
            catch (SuperVException e)
            {
                return TypedResults.InternalServerError<string>(e.Message);
            }
        }

    }
}
