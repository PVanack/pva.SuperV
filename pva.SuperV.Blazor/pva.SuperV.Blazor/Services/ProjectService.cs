using pva.SuperV.Model;
using pva.SuperV.Model.Projects;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Services
{
    public class ProjectService(HttpClient httpClient) : IProjectService
    {
        private const string baseUri = "/projects";
        private const string NoContentAvailableMessage = "No content available";
        private const string NoProjectInResponseMessage = "No project in response";

        public async Task<ProjectModel> BuildProjectAsync(string projectId)
        {
            try
            {
                var result = await httpClient.PostAsync($"{baseUri}/{projectId}/build", null)
                    ?? throw new ApiException(NoContentAvailableMessage);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ProjectModel? project = await result.Content.ReadFromJsonAsync<ProjectModel>();
                    return project ?? throw new ApiException(NoProjectInResponseMessage);
                }
                else
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<ProjectModel> CreateProjectAsync(CreateProjectRequest createProjectRequest)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync($"{baseUri}/create", createProjectRequest)
                    ?? throw new ApiException(NoContentAvailableMessage);
                if (result.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    ProjectModel? project = Task.Run(async () => await result.Content.ReadFromJsonAsync<ProjectModel>()).Result;
                    return project ?? throw new ApiException(NoProjectInResponseMessage);
                }
                else
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<ProjectModel> CreateProjectFromJsonDefinitionAsync(StreamReader streamReader)
        {
            try
            {
                string json = await streamReader.ReadToEndAsync();
                var jsonContent = JsonContent.Create(System.Text.Encoding.UTF8.GetBytes(json));
                var result = await httpClient.PostAsync($"{baseUri}/load-from-definitions", jsonContent)
                    ?? throw new ApiException(NoContentAvailableMessage);
                if (result.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    ProjectModel? project = await result.Content.ReadFromJsonAsync<ProjectModel>();
                    return project ?? throw new ApiException(NoProjectInResponseMessage);
                }
                else
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<ProjectModel> CreateProjectFromRunnableAsync(string runnableProjectId)
        {
            try
            {
                var result = await httpClient.PostAsync($"{baseUri}/create/{runnableProjectId}", null)
                    ?? throw new ApiException(NoContentAvailableMessage);
                if (result.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    ProjectModel? project = await result.Content.ReadFromJsonAsync<ProjectModel>();
                    return project ?? throw new ApiException(NoProjectInResponseMessage);
                }
                else
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<ProjectModel> GetProjectAsync(string projectId)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}")
                    ?? throw new ApiException(NoContentAvailableMessage);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ProjectModel? project = await result.Content.ReadFromJsonAsync<ProjectModel>();
                    return project ?? throw new ApiException(NoProjectInResponseMessage);
                }
                else
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<Stream?> GetProjectDefinitionsAsync(string projectId)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}/definitions")
                    ?? throw new ApiException(NoContentAvailableMessage);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return await result.Content.ReadAsStreamAsync();
                }
                else
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<Stream?> GetProjectInstancesAsync(string projectId)
        {
            try
            {
                var result = await httpClient.GetAsync($"{baseUri}/{projectId}/instances")
                    ?? throw new ApiException(NoContentAvailableMessage);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return await result.Content.ReadAsStreamAsync();
                }
                else
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<List<ProjectModel>> GetProjectsAsync()
        {
            try
            {
                var result = await httpClient.GetAsync(baseUri)
                    ?? throw new ApiException(NoContentAvailableMessage);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<ProjectModel>? projects = await result.Content.ReadFromJsonAsync<List<ProjectModel>>();
                    return projects ?? throw new ApiException(NoProjectInResponseMessage);
                }
                else
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async ValueTask LoadProjectInstancesAsync(string projectId, StreamReader reader)
        {
            try
            {
                string json = await reader.ReadToEndAsync();
                var jsonContent = JsonContent.Create(System.Text.Encoding.UTF8.GetBytes(json));
                var result = await httpClient.PostAsync($"{baseUri}/{projectId}/instances", jsonContent)
                    ?? throw new ApiException(NoContentAvailableMessage);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return;
                }
                else
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<PagedSearchResult<ProjectModel>> SearchProjectsAsync(ProjectPagedSearchRequest search)
        {
            try
            {
                var result = await httpClient.PostAsJsonAsync($"{baseUri}/search", search)
                    ?? throw new ApiException(NoContentAvailableMessage);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    PagedSearchResult<ProjectModel>? projectsPagedSearch = await result.Content.ReadFromJsonAsync<PagedSearchResult<ProjectModel>>();
                    return projectsPagedSearch ?? throw new ApiException(NoProjectInResponseMessage);
                }
                else
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async ValueTask UnloadProjectAsync(string projectId)
        {
            try
            {
                var result = await httpClient.DeleteAsync($"{baseUri}/{projectId}")
                    ?? throw new ApiException(NoContentAvailableMessage);
                if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return;
                }
                else
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }

        public async Task<ProjectModel> UpdateProjectAsync(string projectId, UpdateProjectRequest updateProjectRequest)
        {
            try
            {
                var result = await httpClient.PutAsJsonAsync($"{baseUri}/{projectId}", updateProjectRequest)
                    ?? throw new ApiException(NoContentAvailableMessage);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ProjectModel? project = await result.Content.ReadFromJsonAsync<ProjectModel>();
                    return project ?? throw new ApiException(NoProjectInResponseMessage);
                }
                else
                {
                    throw new ApiException(result.StatusCode, result.Content);
                }
            }
            catch (Exception e)
            {
                throw new ApiException(e);
            }
        }
    }
}
