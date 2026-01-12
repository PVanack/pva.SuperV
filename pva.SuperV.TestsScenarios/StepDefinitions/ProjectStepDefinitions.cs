using pva.Helpers.Extensions;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Model;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.Projects;
using pva.SuperV.TestContainers;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace pva.SuperV.TestsScenarios.StepDefinitions
{

    [Binding]
    public class ProjectStepDefinitions(ScenarioContext scenarioContext) : BaseStepDefinition(scenarioContext)
    {
        private readonly TDengineContainer tdEngineContainer = new();

        [Given("An empty WIP project {string} is created  with {string} description and {string} as history storage")]
        public async ValueTask CreateProject(string projectName, string description, string? historyStorageType)
        {
            string historyStorageConnectionString = "";
            if (!String.IsNullOrEmpty(historyStorageType) && historyStorageType.Equals(TDengineHistoryStorage.Prefix))
            {
                string tdEngineConnectionString = await tdEngineContainer.StartTDengineContainerAsync();
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
            _ = await tdEngineContainer.StopTDengineContainerAsync().ConfigureAwait(false);
        }

        [Then("Getting classes of project {string} returns the following classes")]
        public async ValueTask GettingClassesOfProjectReturnsTheFolowingClasses(string projectId, DataTable expectedClasses)
        {
            var response = await Client.GetAsync($"/classes/{projectId}");

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var classes = await response.Content.ReadFromJsonAsync<List<ClassModel>>();

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            classes.ShouldNotBeNull();
            classes.Count.ShouldBe(expectedClasses.RowCount);
            expectedClasses.Rows.ForEach(row =>
            {
                ClassModel clazz = classes.FirstOrDefault(clazz => clazz.Name!.Equals(row["Name"]!));
                clazz.ShouldNotBeNull();
                string? baseClassName = row["Base class"].IsWhiteSpace() ? null : row["Base class"];
                clazz.BaseClassName.ShouldBe(baseClassName);
            });
        }
        [Then("Searching {string} classes of project {string} returns the following classes")]
        public async ValueTask SearchingClassesOfProjectReturnsTheFolowingClasses(string searchedClasses, string projectId, DataTable expectedClasses)
        {
            ClassPagedSearchRequest search = new(1, 100, searchedClasses, null);
            var response = await Client.PostAsJsonAsync($"/classes/{projectId}/search", search);

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            PagedSearchResult<ClassModel>? classes = await response.Content.ReadFromJsonAsync<PagedSearchResult<ClassModel>>();
            classes.ShouldNotBeNull();
            classes.Result.Count.ShouldBe(expectedClasses.RowCount);
            expectedClasses.Rows.ForEach(row =>
            {
                ClassModel clazz = classes.Result.FirstOrDefault(clazz => clazz.Name!.Equals(row["Name"]!));
                clazz.ShouldNotBeNull();
                string? baseClassName = row["Base class"].IsWhiteSpace() ? null : row["Base class"];
                clazz.BaseClassName.ShouldBe(baseClassName);
            });
        }
    }
}
