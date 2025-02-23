using pva.SuperV.Engine;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Api
{
    public interface IProjectService
    {
        List<ProjectModel> GetProjects();
        ProjectModel GetProject(string projectId);
        ProjectModel CreateProject(CreateProjectRequest createProjectRequest);
        Task<ProjectModel> BuildProjectAsync(string projectId);
        Task<StreamReader?> GetProjectDefinitionsAsync(string projectId);
        void UnloadProject(string projectId);
        ProjectModel CreateProjectFromJsonDefinition(StreamReader streamReader);
    }
}
