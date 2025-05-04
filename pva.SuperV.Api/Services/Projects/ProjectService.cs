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
        private readonly Dictionary<string, Comparison<ProjectModel>> sortOptions = new()
            {
                { "name", new Comparison<ProjectModel>((a, b) => a.Name.CompareTo(b.Name)) }
            };

        public async Task<List<ProjectModel>> GetProjectsAsync()
            => await Task.FromResult(Project.Projects.Values.Select(project => ProjectMapper.ToDto(project)).ToList());

        public async Task<ProjectModel> GetProjectAsync(string projectId)
        {
            return await Task.FromResult(ProjectMapper.ToDto(GetProjectEntity(projectId)));

        }

        public async Task<PagedSearchResult<ProjectModel>> SearchProjectsAsync(ProjectPagedSearchRequest search)
        {
            List<ProjectModel> allProjects = await GetProjectsAsync();
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

        public async Task<ProjectModel> CreateProjectAsync(CreateProjectRequest createProjectRequest)
        {
            WipProject wipProject = Project.CreateProject(createProjectRequest.Name, createProjectRequest.HistoryStorageConnectionString);
            wipProject.Description = createProjectRequest.Description;
            return await Task.FromResult(ProjectMapper.ToDto(wipProject));
        }

        public async Task<ProjectModel> CreateProjectFromRunnableAsync(string runnableProjectId)
        {
            if (GetProjectEntity(runnableProjectId) is RunnableProject runnableProject)
            {
                WipProject wipProject = Project.CreateProject(runnableProject);
                return await Task.FromResult(ProjectMapper.ToDto(wipProject));
            }
            throw new NonRunnableProjectException(runnableProjectId);
        }

        public async Task<ProjectModel> UpdateProjectAsync(string projectId, UpdateProjectRequest updateProjectRequest)
        {
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
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                RunnableProject runnableProject = await Project.BuildAsync(wipProject);
                return ProjectMapper.ToDto(runnableProject);
            }
            throw new NonWipProjectException(projectId);
        }

        public async Task<Stream?> GetProjectDefinitionsAsync(string projectId)
        {
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
            if (GetProjectEntity(projectId) is RunnableProject runnableProject)
            {
                StreamWriter stream = new(new MemoryStream());
                return await ProjectStorage.StreamProjectInstancesAsync(runnableProject, stream);
            }
            throw new NonRunnableProjectException(projectId);
        }

        public async ValueTask LoadProjectInstancesAsync(string projectId, StreamReader reader)
        {
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
            GetProjectEntity(projectId).Unload();
            await ValueTask.CompletedTask;
        }
    }
}
