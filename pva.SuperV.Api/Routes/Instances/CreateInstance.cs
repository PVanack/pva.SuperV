using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Instances;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Instances;

namespace pva.SuperV.Api.Routes.Instances
{
    internal static class CreateInstance
    {
        internal static Results<Created<InstanceModel>, NotFound<string>, BadRequest<string>> Handle(IInstanceService instanceService, string projectId, InstanceModel createRequest)
        {
            try
            {
                return TypedResults.Created($"/instances/{projectId}/{createRequest.Name}", instanceService.CreateInstance(projectId, createRequest));
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