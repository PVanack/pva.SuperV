using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.Projects;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class SearchProjects
    {
        internal static async Task<Results<Ok<PagedSearchResult<ProjectModel>>, BadRequest<string>>>
            Handle(IProjectService projectService, ProjectPagedSearchRequest search)
        {
            try
            {
                PagedSearchResult<ProjectModel> projects = await projectService.SearchProjectsAsync(search);
                return TypedResults.Ok(projects);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}