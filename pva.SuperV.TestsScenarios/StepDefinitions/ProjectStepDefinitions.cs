using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Model.Projects;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace pva.SuperV.TestsScenarios.StepDefinitions
{
    [Binding]
    public class ProjectStepDefinitions(ScenarioContext scenarioContext) : BaseStepDefinition(scenarioContext)
    {
        [Given("An empty WIP project {string} is created  with {string} description and {string} as history storage")]
        public async ValueTask CreateProject(string projectName, string description, string? historyStorageType)
        {
            string historyStorageConnectionString = "";
            if (!String.IsNullOrEmpty(historyStorageType) && historyStorageType.Equals(TDengineHistoryStorage.Prefix))
            {
                // TODO: Start TDengine container
                string tdEngineConnectionString = await StartTDengineContainerAsync();
                historyStorageConnectionString = $"{TDengineHistoryStorage.Prefix}:{tdEngineConnectionString}";

            }
            var response = await Client.PostAsJsonAsync("/projects/create", new CreateProjectRequest(projectName, description, historyStorageConnectionString));

            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            response.Content.ShouldNotBeNull();

            ProjectModel? createdProject = await response.Content.ReadFromJsonAsync<ProjectModel>();
            createdProject.ShouldNotBeNull();
        }

        [Given("Runnable project is built from WIP project {string}")]
        public async ValueTask BuildRunnableProjectFromWipProject(string wipProjectId)
        {
            var response = await Client.PostAsync($"/projects/{wipProjectId}/build", null);

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            ProjectModel? builtProject = await response.Content.ReadFromJsonAsync<ProjectModel>();
            builtProject.ShouldNotBeNull();
        }

        [StepDefinition("TDengine is stopped if running")]
        public async ValueTask ThenTDengineIsStoppedIfRunning()
        {
            _ = await StopTDengineContainerAsync().ConfigureAwait(false);
        }

    }
}
