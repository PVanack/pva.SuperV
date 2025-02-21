using pva.SuperV.Engine;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Api
{
    public class ProjectService : BaseService, IProjectService
    {
        public List<ProjectModel> GetProjects()
            => Project.Projects.Values
                .Select(project => ProjectMapper.ToDto(project))
                .ToList();

        public ProjectModel GetProject(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            return ProjectMapper.ToDto(project);

        }

        public ProjectModel CreateProject(CreateProjectRequest createProjectRequest)
        {
            WipProject wipProject = Project.CreateProject(createProjectRequest.Name);
            wipProject.Description = createProjectRequest.Description;
            return ProjectMapper.ToDto(wipProject);
        }

        public ProjectModel BuildProject(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            if (project is WipProject wipProject)
            {
                RunnableProject runnableProject = Project.Build(wipProject);
                return ProjectMapper.ToDto(runnableProject);
            }
            throw new NonWipProjectException(projectId);
        }

        public string GetProjectDefinitions(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            return ProjectStorage.GetProjectDefinition(project);
        }

        public ProjectModel LoadProjectDefinitions(StreamReader streamReader)
        {
            return ProjectMapper.ToDto(ProjectStorage.LoadProjectDefinition<Project>(streamReader));
        }

        public void UnloadProject(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            project.Unload();
        }
    }
}
