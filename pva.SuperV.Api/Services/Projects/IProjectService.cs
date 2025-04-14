using pva.SuperV.Model;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Api.Services.Projects
{
    public interface IProjectService
    {
        List<ProjectModel> GetProjects();
        ProjectModel GetProject(string projectId);
        ProjectModel CreateProject(CreateProjectRequest createProjectRequest);
        ProjectModel CreateProjectFromRunnable(string runnableProjectId);
        Task<ProjectModel> BuildProjectAsync(string projectId);
        Task<StreamReader?> GetProjectDefinitionsAsync(string projectId);
        void UnloadProject(string projectId);
        ProjectModel CreateProjectFromJsonDefinition(StreamReader streamReader);
        Task<StreamReader?> GetProjectInstancesAsync(string projectId);
        void LoadProjectInstances(string projectId, StreamReader reader);
        ProjectModel UpdateProject(string projectId, UpdateProjectRequest updateProjectRequest);
        PagedSearchResult<ProjectModel> SearchProjects(ProjectPagedSearchRequest search);
    }
}
