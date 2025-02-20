using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldFormatters;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    public static class CreateFieldFormatter
    {

        internal static Results<Created<FieldFormatterModel>, InternalServerError<string>> Handle(IFieldFormatterService fieldFormatterService, string projectId, CreateFieldFormatterRequest createRequest)
        {
            try
            {
                FieldFormatterModel createdFieldFormatter = fieldFormatterService.CreateFieldFormatter(projectId, createRequest.FieldFormatter);
                return TypedResults.Created<FieldFormatterModel>($"/field-formatters/{projectId}/{createdFieldFormatter.Name}", createdFieldFormatter);
            }
            catch (SuperVException e)
            {
                return TypedResults.InternalServerError<string>(e.Message);
            }
        }
    }
}