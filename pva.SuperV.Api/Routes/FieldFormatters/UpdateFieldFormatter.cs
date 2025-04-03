using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.FieldFormatters;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldFormatters;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    internal static class UpdateFieldFormatter
    {
        internal static Results<Ok<FieldFormatterModel>, NotFound<string>, BadRequest<string>> Handle(IFieldFormatterService fieldFormatterService,
            string wipProjectId, string fieldFormatterName, FieldFormatterModel fieldFormatterModel)
        {
            try
            {
                FieldFormatterModel updatedFieldFormatter = fieldFormatterService.UpdateFieldFormatter(wipProjectId, fieldFormatterName, fieldFormatterModel);
                return TypedResults.Ok<FieldFormatterModel>(updatedFieldFormatter);
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