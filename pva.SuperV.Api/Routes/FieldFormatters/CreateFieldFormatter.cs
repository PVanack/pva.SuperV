using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldFormatters;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    internal static class CreateFieldFormatter
    {
        internal static async Task<Results<Created<FieldFormatterModel>, NotFound<string>, BadRequest<string>>>
            Handle(IFieldFormatterService fieldFormatterService, string projectId, CreateFieldFormatterRequest createRequest)
        {
            try
            {
                FieldFormatterModel createdFieldFormatter = await fieldFormatterService.CreateFieldFormatterAsync(projectId, createRequest);
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