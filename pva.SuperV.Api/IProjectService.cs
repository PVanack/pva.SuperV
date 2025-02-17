using pva.SuperV.Model;

namespace pva.SuperV.Api
{
    public interface IProjectService
    {
        List<ProjectModel> GetProjects();
        ProjectModel GetProject(string projectId);
        ProjectModel CreateProject(CreateProjectRequest createProjectRequest);
        ProjectModel BuildProject(string projectId);

        List<ClassModel> GetClasses(string projectId);
        ClassModel GetClass(string projectId, string className);
    }
}
