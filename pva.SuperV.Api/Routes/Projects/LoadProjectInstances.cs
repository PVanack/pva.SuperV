
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class LoadProjectInstances
    {
        internal static Results<Ok, NotFound<string>, InternalServerError<string>> Handle(IProjectService projectService, string projectId, HttpRequest request)
        {
            try
            {
                using StreamReader reader = new(request.Body, System.Text.Encoding.UTF8);
                projectService.LoadProjectInstances(projectId, reader);
                return TypedResults.Ok();
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.InternalServerError(e.Message);
            }
        }
    }
}