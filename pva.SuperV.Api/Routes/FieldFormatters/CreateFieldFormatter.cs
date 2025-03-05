using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.FieldFormatters;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldFormatters;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    public static class CreateFieldFormatter
    {

        internal static Results<Created<FieldFormatterModel>, NotFound<string>, BadRequest<string>> Handle(IFieldFormatterService fieldFormatterService, string projectId, CreateFieldFormatterRequest createRequest)
        {
            try
            {
                FieldFormatterModel createdFieldFormatter = fieldFormatterService.CreateFieldFormatter(projectId, createRequest.FieldFormatter);
                return TypedResults.Created<FieldFormatterModel>($"/field-formatters/{projectId}/{createdFieldFormatter.Name}", createdFieldFormatter);
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