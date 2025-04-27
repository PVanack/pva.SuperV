using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.FieldDefinitions
{
    internal static class CreateFieldDefinitions
    {
        internal static async Task<Results<Created<List<FieldDefinitionModel>>, NotFound<string>, BadRequest<string>>>
            Handle(IFieldDefinitionService fieldDefinitionService, string projectId, string className, List<FieldDefinitionModel> createRequests)
        {
            try
            {
                List<FieldDefinitionModel> createdFieldDefinitions = await fieldDefinitionService.CreateFieldsAsync(projectId, className, createRequests);
                return TypedResults.Created(null as Uri, createdFieldDefinitions);
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