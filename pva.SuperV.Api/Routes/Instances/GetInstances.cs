using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Instances;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Instances;

namespace pva.SuperV.Api.Routes.Instances
{
    internal static class GetInstances
    {
        public static Results<Ok<List<InstanceModel>>, NotFound<string>, BadRequest<string>> Handle(IInstanceService instanceService, string projectId)
        {
            try
            {
                return TypedResults.Ok(instanceService.GetInstances(projectId));
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