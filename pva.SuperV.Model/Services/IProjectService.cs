using pva.SuperV.Model.Projects;

namespace pva.SuperV.Model.Services
{
    public interface IProjectService
    {
        Task<List<ProjectModel>> GetProjectsAsync();
        Task<ProjectModel> GetProjectAsync(string projectId);
        Task<ProjectModel> CreateProjectAsync(CreateProjectRequest createProjectRequest);
        Task<ProjectModel> CreateProjectFromRunnableAsync(string runnableProjectId);
        Task<ProjectModel> BuildProjectAsync(string projectId);
        Task<Stream?> GetProjectDefinitionsAsync(string projectId);
        ValueTask UnloadProjectAsync(string projectId);
        Task<ProjectModel> CreateProjectFromJsonDefinitionAsync(StreamReader streamReader);
        Task<Stream?> GetProjectInstancesAsync(string projectId);
        ValueTask LoadProjectInstancesAsync(string projectId, StreamReader reader);
        Task<ProjectModel> UpdateProjectAsync(string projectId, UpdateProjectRequest updateProjectRequest);
        Task<PagedSearchResult<ProjectModel>> SearchProjectsAsync(ProjectPagedSearchRequest search);
        Task<HashSet<string>> GetProjectTopicNames(string projectId);
    }
}
