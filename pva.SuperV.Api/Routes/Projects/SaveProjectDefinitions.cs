
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class SaveProjectDefinitions
    {
        internal static Results<ContentHttpResult, NotFound<string>, InternalServerError<string>> Handle(IProjectService projectService, string projectId)
        {
            try
            {
                return TypedResults.Text(projectService.GetProjectDefinitions(projectId));
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.InternalServerError<string>(e.Message);
            }
        }
    }
}