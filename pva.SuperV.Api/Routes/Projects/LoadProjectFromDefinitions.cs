
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class LoadProjectFromDefinitions
    {
        internal static Results<Created<ProjectModel>, NotFound<string>, InternalServerError<string>> Handle(IProjectService projectService, HttpRequest request)
        {
            try
            {
                using StreamReader reader = new(request.Body, System.Text.Encoding.UTF8);
                ProjectModel projectModel = projectService.CreateProjectFromJsonDefinition(reader);
                return TypedResults.Created($"/projects/{projectModel.Id}", projectModel);
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