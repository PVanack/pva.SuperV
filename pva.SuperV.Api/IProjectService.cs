using pva.SuperV.Engine;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Api
{
    public interface IProjectService
    {
        List<ProjectModel> GetProjects();
        ProjectModel GetProject(string projectId);
        ProjectModel CreateProject(CreateProjectRequest createProjectRequest);
        ProjectModel BuildProject(string projectId);
        string GetProjectDefinitions(string projectId);
        void UnloadProject(string projectId);
        ProjectModel LoadProjectDefinitions(StreamReader streamReader);
    }
}
