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
            WipProject wipProject = Project.CreateProject(createProjectRequest.Name, createProjectRequest.HistoryStorageConnectionString);
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

        public async Task<StreamReader?> GetProjectInstancesAsync(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            if (project is RunnableProject runnableProject)
            {
                StreamWriter stream = new(new MemoryStream());
                return await ProjectStorage.StreamProjectInstances(runnableProject, stream);
            }
            throw new NonRunnableProjectException(projectId);
        }

        public void LoadProjectInstances(string projectId, StreamReader reader)
        {
            Project project = GetProjectEntity(projectId);
            if (project is RunnableProject runnableProject)
            {
                ProjectStorage.LoadProjectInstances(runnableProject, reader);
                return;
            }
            throw new NonRunnableProjectException(projectId);
        }

        public void UnloadProject(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            project.Unload();
        }
    }
}
