using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Api.Services.Projects
{
    public class ProjectService : BaseService, IProjectService
    {
        private readonly Dictionary<string, Comparison<ProjectModel>> sortOptions = new()
            {
                { "name", new Comparison<ProjectModel>((a, b) => a.Name.CompareTo(b.Name)) }
            };

        public List<ProjectModel> GetProjects()
            => [.. Project.Projects.Values.Select(project => ProjectMapper.ToDto(project))];

        public ProjectModel GetProject(string projectId)
        {
            return ProjectMapper.ToDto(GetProjectEntity(projectId));

        }

        public PagedSearchResult<ProjectModel> SearchProjects(ProjectPagedSearchRequest search)
        {
            List<ProjectModel> allProjects = GetProjects();
            List<ProjectModel> projects = FilterProjects(allProjects, search);
            projects = SortResult(projects, search.SortOption, sortOptions);
            return CreateResult(search, allProjects, projects);
        }

        private static List<ProjectModel> FilterProjects(List<ProjectModel> allProjects, ProjectPagedSearchRequest search)
        {
            List<ProjectModel> filteredProjects = allProjects;
            if (!String.IsNullOrEmpty(search.NameFilter))
            {
                filteredProjects = [.. filteredProjects.Where(project => project.Name.Contains(search.NameFilter))];
            }
            return filteredProjects;
        }

        public ProjectModel CreateProject(CreateProjectRequest createProjectRequest)
        {
            WipProject wipProject = Project.CreateProject(createProjectRequest.Name, createProjectRequest.HistoryStorageConnectionString);
            wipProject.Description = createProjectRequest.Description;
            return ProjectMapper.ToDto(wipProject);
        }

        public ProjectModel CreateProjectFromRunnable(string runnableProjectId)
        {
            if (GetProjectEntity(runnableProjectId) is RunnableProject runnableProject)
            {
                WipProject wipProject = Project.CreateProject(runnableProject);
                return ProjectMapper.ToDto(wipProject);
            }
            throw new NonRunnableProjectException(runnableProjectId);
        }

        public ProjectModel UpdateProject(string projectId, UpdateProjectRequest updateProjectRequest)
        {
            Project? projectToUpdate = GetProjectEntity(projectId);
            if (projectToUpdate is not null)
            {
                projectToUpdate.Description = updateProjectRequest.Description;
                projectToUpdate.HistoryStorageEngineConnectionString = updateProjectRequest.HistoryStorageConnectionString;
                return ProjectMapper.ToDto(projectToUpdate);
            }
            throw new UnknownEntityException("Project", projectId);
        }

        public async Task<ProjectModel> BuildProjectAsync(string projectId)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
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
            return await ProjectStorage.StreamProjectDefinitionAsync(project, stream);
        }

        public ProjectModel CreateProjectFromJsonDefinition(StreamReader streamReader)
        {
            return ProjectMapper.ToDto(ProjectStorage.CreateProjectFromJsonDefinition<RunnableProject>(streamReader));
        }

        public async Task<StreamReader?> GetProjectInstancesAsync(string projectId)
        {
            if (GetProjectEntity(projectId) is RunnableProject runnableProject)
            {
                StreamWriter stream = new(new MemoryStream());
                return await ProjectStorage.StreamProjectInstancesAsync(runnableProject, stream);
            }
            throw new NonRunnableProjectException(projectId);
        }

        public void LoadProjectInstances(string projectId, StreamReader reader)
        {
            if (GetProjectEntity(projectId) is RunnableProject runnableProject)
            {
                ProjectStorage.LoadProjectInstances(runnableProject, reader);
                return;
            }
            throw new NonRunnableProjectException(projectId);
        }

        public void UnloadProject(string projectId)
        {
            GetProjectEntity(projectId).Unload();
        }
    }
}
