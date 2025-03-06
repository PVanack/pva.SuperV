using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.FieldDefinitions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldDefinitions;

namespace pva.SuperV.Api.Routes.FieldDefinitions
{
    internal static class CreateField
    {
        internal static Results<Created, NotFound<string>, BadRequest<string>>
            Handle(IFieldDefinitionService fieldDefinitionService, string projectId, string className, List<FieldDefinitionModel> createRequests)
        {
            try
            {
                fieldDefinitionService.CreateFields(projectId, className, createRequests);
                return TypedResults.Created();
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