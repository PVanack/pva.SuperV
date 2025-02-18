using pva.SuperV.Model;

namespace pva.SuperV.Api
{
    public interface IProjectService
    {
        List<ProjectModel> GetProjects();
        ProjectModel GetProject(string projectId);
        ProjectModel CreateProject(CreateProjectRequest createProjectRequest);
        ProjectModel BuildProject(string projectId);
    }
}
