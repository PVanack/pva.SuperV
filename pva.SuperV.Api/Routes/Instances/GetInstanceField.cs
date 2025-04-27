using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Instances;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Instances
{
    internal static class GetInstanceField
    {
        internal static async Task<Results<Ok<FieldModel>, NotFound<string>, BadRequest<string>>>
            Handle(IFieldValueService fieldValueService, string projectId, string instanceName, string fieldName)
        {
            try
            {
                return TypedResults.Ok(await fieldValueService.GetFieldAsync(projectId, instanceName, fieldName));
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