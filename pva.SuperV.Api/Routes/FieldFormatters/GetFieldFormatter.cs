using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.FieldFormatters;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldFormatters;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    public static class GetFieldFormatter
    {
        internal static Results<Ok<FieldFormatterModel>, NotFound<string>, BadRequest<string>> Handle(IFieldFormatterService fieldFormatterService, string projectId, string fieldFormatterName)
        {
            try
            {
                FieldFormatterModel fieldFormatter = fieldFormatterService.GetFieldFormatter(projectId, fieldFormatterName);
                return TypedResults.Ok<FieldFormatterModel>(fieldFormatter);
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
