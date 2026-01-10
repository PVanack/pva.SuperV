using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.Projects;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Services.Projects
{
    public class ProjectService : BaseService, IProjectService
    {
        private readonly ILogger logger;
        private readonly Dictionary<string, Comparison<ProjectModel>> sortOptions = new()
            {
                { "name", new Comparison<ProjectModel>((a, b) => a.Name.CompareTo(b.Name)) }
            };

        public ProjectService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(this.GetType());
        }

        public async Task<List<ProjectModel>> GetProjectsAsync()
        {
            logger.LogDebug("Getting projects");
            return await Task.FromResult(Project.Projects.Values.Select(project => ProjectMapper.ToDto(project)).ToList());
        }

        public async Task<ProjectModel> GetProjectAsync(string projectId)
        {
            logger.LogDebug("Getting project {ProjectId}",
                projectId);
            return await Task.FromResult(ProjectMapper.ToDto(GetProjectEntity(projectId)));
        }

        public async Task<PagedSearchResult<ProjectModel>> SearchProjectsAsync(ProjectPagedSearchRequest search)
        {
            logger.LogDebug("Searching projects with filter {NameFilter} page number {PageNumber} page size {PageSize}",
                search.NameFilter, search.PageNumber, search.PageSize);
            List<ProjectModel> allProjects = await GetProjectsAsync();
            List<ProjectModel> projects = FilterProjects(allProjects, search);
            projects = SortResult(projects, search.SortOption, sortOptions);
            return CreateResult(search, allProjects, projects);
        }

        public async Task<ProjectModel> CreateProjectAsync(CreateProjectRequest createProjectRequest)
        {
            logger.LogDebug("Creating project {ProjectId} with description {ProjectDescription} with history connection {HistoryConnectionString}",
                createProjectRequest.Name, createProjectRequest.Description, createProjectRequest.HistoryStorageConnectionString);
            WipProject wipProject = Project.CreateProject(createProjectRequest.Name, createProjectRequest.HistoryStorageConnectionString);
            wipProject.Description = createProjectRequest.Description;
            return await Task.FromResult(ProjectMapper.ToDto(wipProject));
        }

        public async Task<ProjectModel> CreateProjectFromRunnableAsync(string runnableProjectId)
        {
            logger.LogDebug("Creating project {ProjectId} from runnable",
                runnableProjectId);
            if (GetProjectEntity(runnableProjectId) is RunnableProject runnableProject)
            {
                WipProject wipProject = Project.CreateProject(runnableProject);
                return await Task.FromResult(ProjectMapper.ToDto(wipProject));
            }
            throw new NonRunnableProjectException(runnableProjectId);
        }

        public async Task<ProjectModel> UpdateProjectAsync(string projectId, UpdateProjectRequest updateProjectRequest)
        {
            logger.LogDebug("Updating project {ProjectId} with description {ProjectDescription} with history connection {HistoryConnectionString}",
                projectId, updateProjectRequest.Description, updateProjectRequest.HistoryStorageConnectionString);
            Project? projectToUpdate = GetProjectEntity(projectId);
            if (projectToUpdate is not null)
            {
                projectToUpdate.Description = updateProjectRequest.Description;
                projectToUpdate.HistoryStorageEngineConnectionString = updateProjectRequest.HistoryStorageConnectionString;
                return await Task.FromResult(ProjectMapper.ToDto(projectToUpdate));
            }
            return await Task.FromException<ProjectModel>(new UnknownEntityException("Project", projectId));
        }

        public async Task<ProjectModel> BuildProjectAsync(string projectId)
        {
            logger.LogDebug("Building project {ProjectId}",
                projectId);
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                RunnableProject runnableProject = await Project.BuildAsync(wipProject);
                return ProjectMapper.ToDto(runnableProject);
            }
            throw new NonWipProjectException(projectId);
        }

        public async Task<Stream?> GetProjectDefinitionsAsync(string projectId)
        {
            logger.LogDebug("Getting project {ProjectId} definitions",
                projectId);
            Project project = GetProjectEntity(projectId);
            StreamWriter stream = new(new MemoryStream());
            return await ProjectStorage.StreamProjectDefinitionAsync(project, stream);
        }

        public async Task<ProjectModel> CreateProjectFromJsonDefinitionAsync(StreamReader streamReader)
        {
            return await Task.FromResult(ProjectMapper.ToDto(ProjectStorage.CreateProjectFromJsonDefinition<RunnableProject>(streamReader)));
        }

        public async Task<Stream?> GetProjectInstancesAsync(string projectId)
        {
            logger.LogDebug("Getting project {ProjectId} instances",
                projectId);
            if (GetProjectEntity(projectId) is RunnableProject runnableProject)
            {
                StreamWriter stream = new(new MemoryStream());
                return await ProjectStorage.StreamProjectInstancesAsync(runnableProject, stream);
            }
            throw new NonRunnableProjectException(projectId);
        }

        public async ValueTask LoadProjectInstancesAsync(string projectId, StreamReader reader)
        {
            logger.LogDebug("Loading project {ProjectId} instances",
                projectId);
            if (GetProjectEntity(projectId) is RunnableProject runnableProject)
            {
                ProjectStorage.LoadProjectInstances(runnableProject, reader);
                await ValueTask.CompletedTask;
                return;
            }
            await ValueTask.FromException(new NonRunnableProjectException(projectId));
        }

        public async ValueTask UnloadProjectAsync(string projectId)
        {
            logger.LogDebug("Unloading project {ProjectId}",
                projectId);
            GetProjectEntity(projectId).Unload();
            await ValueTask.CompletedTask;
        }

        public async Task<HashSet<string>> GetProjectTopicNames(string projectId)
        {
            logger.LogDebug("Getting project {ProjectId} topic names",
                projectId);
            Project? project = GetProjectEntity(projectId);
            if (project is not null)
            {
                return await Task.FromResult(project.GetTopicNames());
            }
            return await Task.FromException<HashSet<string>>(new UnknownEntityException("Project", projectId));

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
    }
}
