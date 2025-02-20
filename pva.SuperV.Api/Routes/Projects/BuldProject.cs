using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Api.Routes.Projects
{
    public static class BuildProject
    {
        internal static Results<Ok<ProjectModel>, NotFound<string>, BadRequest<string>, InternalServerError<string>> Handle(IProjectService projectService, string projectId)
        {
            try
            {
                ProjectModel createdProject = projectService.BuildProject(projectId);
                return TypedResults.Ok<ProjectModel>(createdProject);
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound<string>(e.Message);
            }
            catch (NonWipProjectException e)
            {
                return TypedResults.BadRequest<string>(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.InternalServerError<string>(e.Message);
            }
        }
    }
}