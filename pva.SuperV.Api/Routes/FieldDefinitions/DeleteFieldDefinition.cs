using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.FieldDefinitions
{
    internal static class DeleteFieldDefinition
    {
        internal static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>>
            Handle(IFieldDefinitionService fieldDefinitionService, string projectId, string className, string fieldName)
        {
            try
            {
                await fieldDefinitionService.DeleteFieldAsync(projectId, className, fieldName);
                return TypedResults.NoContent();
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound<string>(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest<string>(e.Message);
            }
        }
    }
}