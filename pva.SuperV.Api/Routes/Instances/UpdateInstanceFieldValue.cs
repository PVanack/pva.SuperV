using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Instances;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Instances;

namespace pva.SuperV.Api.Routes.Instances
{
    internal static class UpdateInstanceFieldValue
    {
        internal static Results<Ok<FieldValueModel>, NotFound<string>, BadRequest<string>> Handle(IFieldValueService fieldValueService, string projectId, string instanceName, string fieldName, FieldValueModel value)
        {
            try
            {
                return TypedResults.Ok(fieldValueService.UpdateFieldValue(projectId, instanceName, fieldName, value));
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