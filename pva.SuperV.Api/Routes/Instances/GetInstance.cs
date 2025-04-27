using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Instances;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Instances
{
    internal static class GetInstance
    {
        internal static async Task<Results<Ok<InstanceModel>, NotFound<string>, BadRequest<string>>>
            Handle(IInstanceService instanceService, string projectId, string instanceName)
        {
            try
            {
                return TypedResults.Ok(await instanceService.GetInstanceAsync(projectId, instanceName));
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