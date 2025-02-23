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

        public async Task<ProjectModel> BuildProjectAsync(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            if (project is WipProject wipProject)
            {
                RunnableProject runnableProject = await Project.BuildAsync(wipProject);
                return ProjectMapper.ToDto(runnableProject);
            }
            throw new NonWipProjectException(projectId);
        }

        public async Task<StreamReader?> GetProjectDefinitionsAsync(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            StreamWriter stream = new(new MemoryStream());
            return await ProjectStorage.StreamProjectDefinition(project, stream);
        }

        public ProjectModel CreateProjectFromJsonDefinition(StreamReader streamReader)
        {
            return ProjectMapper.ToDto(ProjectStorage.CreateProjectFromJsonDefinition<RunnableProject>(streamReader));
        }

        public void UnloadProject(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            project.Unload();
        }
    }
}
