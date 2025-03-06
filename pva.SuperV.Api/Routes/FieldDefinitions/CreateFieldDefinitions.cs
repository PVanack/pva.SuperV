using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.FieldDefinitions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldDefinitions;

namespace pva.SuperV.Api.Routes.FieldDefinitions
{
    internal static class CreateFieldDefinitions
    {
        internal static Results<Created<List<FieldDefinitionModel>>, NotFound<string>, BadRequest<string>>
            Handle(IFieldDefinitionService fieldDefinitionService, string projectId, string className, List<FieldDefinitionModel> createRequests)
        {
            try
            {
                List<FieldDefinitionModel> createdFieldDefinitions = fieldDefinitionService.CreateFields(projectId, className, createRequests);
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