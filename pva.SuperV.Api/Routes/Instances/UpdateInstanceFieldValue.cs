using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Instances;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Instances
{
    internal static class UpdateInstanceFieldValue
    {
        internal static async Task<Results<Ok<FieldValueModel>, NotFound<string>, BadRequest<string>>>
            Handle(IFieldValueService fieldValueService, string projectId, string instanceName, string fieldName, FieldValueModel value)
        {
            try
            {
                return TypedResults.Ok(await fieldValueService.UpdateFieldValueAsync(projectId, instanceName, fieldName, value));
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