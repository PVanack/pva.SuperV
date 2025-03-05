using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Instances;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Api.Routes.Instances
{
    internal static class DeleteInstance
    {
        internal static Results<NoContent, NotFound<string>, BadRequest<string>> Handle(IInstanceService instanceService, string projectId, string instanceName)
        {
            try
            {
                instanceService.DeleteInstance(projectId, instanceName);
                return TypedResults.NoContent();
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