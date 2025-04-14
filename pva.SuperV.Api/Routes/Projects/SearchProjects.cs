using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Projects;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class SearchProjects
    {
        internal static Results<Ok<PagedSearchResult<ProjectModel>>, BadRequest<string>> Handle(IProjectService projectService, ProjectPagedSearchRequest search)
        {
            try
            {
                PagedSearchResult<ProjectModel> projects = projectService.SearchProjects(search);
                return TypedResults.Ok(projects);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}