using NSubstitute;
using NSubstitute.ExceptionExtensions;
using pva.SuperV.Api.Exceptions;
using pva.SuperV.Api.Services.Projects;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.Projects;
using Shouldly;
using System.Net.Http.Json;
using System.Text;
using Xunit.Abstractions;

namespace pva.SuperV.ApiTests
{
    public class ProjectEndpointsTests
    {
        public class ConsoleWriter(ITestOutputHelper output) : StringWriter
        {
            public override void WriteLine(string? value) => output.WriteLine(value);
        }

        private const string projectDefinitionsJson = """
                {
                    "Name": "TestProject",
                    "Description": null,
                    "Version": 11,
                    "Classes": {
                        "TestClass": {
                            "Name": "TestClass",
                            "BaseClassName": null,
                            "FieldDefinitions": {
                                "Value": {
                                    "Type": "System.DateTime",
                                    "Name": "Value",
                                    "DefaultValue": "0001-01-01T00:00:00",
                                    "ValuePostChangeProcessings": []
                                }
                            }
                        }
                    },
                    "FieldFormatters": {},
                    "HistoryStorageEngineConnectionString": null,
                    "HistoryRepositories": {}
                }
                """;
        private const string projectInstancesJson = """
            {
                "Instance": {
                    "Class": "TestClass",
                    "Name": "Instance",
                    "Fields": [
                        {
                            "Type": "System.Int32",
                            "Name": "Value",
                            "Value": 12,
                            "Timestamp": "2025-02-22T12:52:19.19Z",
                            "Quality": "Good"
                        }
                    ]
                }
            }
            """;
        private const string createProjectInstancesJson = """
            {
                "Instance": {
                    "Class": "TestClass",
                    "Name": "Instance2",
                    "Fields": [
                        {
                            "Type": "System.Int32",
                            "Name": "Value",
                            "Value": 12,
                            "Timestamp": "2025-02-22T12:52:19.19Z",
                            "Quality": "Good"
                        }
                    ]
                }
            }
            """;
        private readonly TestProjectApplication application;
        private readonly HttpClient client;

        private IProjectService MockedProjectService { get => application.MockedProjectService!; }

        public ProjectEndpointsTests(ITestOutputHelper output)
        {
            application = new();
            client = application.CreateClient();
            Console.SetOut(new ConsoleWriter(output));
        }

        [Fact]
        public async Task GivenExistingProjects_WhenSearchingProjects_ThenProjectsAreReturned()
        {
            // GIVEN
            List<ProjectModel> expectedProjects = [new ProjectModel("Project1", "a", 1, "Descr", false)];
            ProjectPagedSearchRequest searchRequest = new(1, 10, null, null);
            MockedProjectService.SearchProjects(Arg.Any<ProjectPagedSearchRequest>())
                .Returns(new PagedSearchResult<ProjectModel>(1, 10, expectedProjects.Count, expectedProjects));

            // WHEN
            var response = await client.PostAsJsonAsync("/projects/search", searchRequest);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            PagedSearchResult<ProjectModel>? projectSearchResult = await response.Content.ReadFromJsonAsync<PagedSearchResult<ProjectModel>>();
            projectSearchResult.ShouldNotBeNull();
            projectSearchResult.PageNumber.ShouldBe(1);
            projectSearchResult.PageSize.ShouldBe(10);
            projectSearchResult.Count.ShouldBe(1);
            projectSearchResult.Result.ShouldBeEquivalentTo(expectedProjects);
        }

        [Fact]
        public async Task GivenExistingProjects_WhenGettingProjects_ThenProjectsAreReturned()
        {
            // GIVEN
            List<ProjectModel> expectedProjects = [new ProjectModel("Project1", "a", 1, "Descr", false)];
            MockedProjectService.GetProjects()
                .Returns(expectedProjects);

            // WHEN
            var response = await client.GetAsync("/projects");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            ProjectModel[]? projects = await response.Content.ReadFromJsonAsync<ProjectModel[]>();
            projects.ShouldBeEquivalentTo(expectedProjects.ToArray());
        }

        [Fact]
        public async Task GivenExistingProject_WhenGettingExistingProject_ThenProjectIsReturned()
        {
            // GIVEN
            ProjectModel expectedProject = new("Project1", "Project1", 1, "Descr", false);
            MockedProjectService.GetProject("Project1")
                .Returns(expectedProject);

            // WHEN
            var response = await client.GetAsync("/projects/Project1");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            ProjectModel? project = await response.Content.ReadFromJsonAsync<ProjectModel>();
            project.ShouldBeEquivalentTo(expectedProject);
        }

        [Fact]
        public async Task WhenGettingUnknownProject_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedProjectService.GetProject("UnknownProject")
                .Throws<UnknownEntityException>();

            // WHEN
            var response = await client.GetAsync("/projects/UnknownProject");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenNoProjects_WhenCreatingNewProject_ThenProjectIsCreated()
        {
            // GIVEN
            CreateProjectRequest createProjectRequest = new("NewProject", "Description");
            ProjectModel expectedCreatedProject = new("1", "NewProject", 1, "descriptioon", false);
            MockedProjectService.CreateProject(createProjectRequest)
                .Returns(expectedCreatedProject);

            // WHEN
            var response = await client.PostAsJsonAsync("/projects/create", createProjectRequest);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            var createdProject = await response.Content.ReadFromJsonAsync<ProjectModel>();
            createdProject.ShouldBeEquivalentTo(expectedCreatedProject);
        }

        [Fact]
        public async Task GivenExistingProject_WhenUpdatingProject_ThenProjectIsUpdated()
        {
            // GIVEN
            UpdateProjectRequest updateProjectRequest = new("NewProject", "Description");
            ProjectModel expectedUpdatedProject = new("1", "NewProject", 1, "descriptioon", false);
            MockedProjectService.UpdateProject(expectedUpdatedProject.Name, updateProjectRequest)
                .Returns(expectedUpdatedProject);

            // WHEN
            var response = await client.PutAsJsonAsync($"/projects/{expectedUpdatedProject.Name}", updateProjectRequest);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            var updatedProject = await response.Content.ReadFromJsonAsync<ProjectModel>();
            updatedProject.ShouldBeEquivalentTo(expectedUpdatedProject);
        }

        [Fact]
        public async Task GivenRunnableProject_WhenCreatingWipProjectFromRunnable_ThenWipProjectIsCreated()
        {
            // GIVEN
            ProjectModel expectedCreatedProject = new("1", "NewProject", 2, "descriptioon", false);
            MockedProjectService.CreateProjectFromRunnable("NewProject")
                .Returns(expectedCreatedProject);

            // WHEN
            var response = await client.PostAsync("/projects/create/NewProject", null);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            var createdProject = await response.Content.ReadFromJsonAsync<ProjectModel>();
            createdProject.ShouldBeEquivalentTo(expectedCreatedProject);
        }

        [Fact]
        public async Task WhenCreatingWipProjectFromWipProject_ThenBadRequestIsReturned()
        {
            // GIVEN
            MockedProjectService.CreateProjectFromRunnable("WipProject")
                .Throws<NonRunnableProjectException>();

            // WHEN
            var response = await client.PostAsync("/projects/create/WipProject", null);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenWipProject_WhenBuildigProjectFromIt_ThenRunnableProjectIsBuilt()
        {
            // GIVEN
            ProjectModel expectedRunnableProject = new("Project-Wip", "a", 2, "description", true);
            MockedProjectService.BuildProjectAsync("Project-Wip")
                .Returns(expectedRunnableProject);

            // WHEN
            var response = await client.PostAsync("/projects/Project-Wip/build", null);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            var createdProject = await response.Content.ReadFromJsonAsync<ProjectModel>();
            createdProject.ShouldBeEquivalentTo(expectedRunnableProject);
        }

        [Fact]
        public async Task WhenBuildigProjectFromRunnableProject_ThenBadRequestIsReturned()
        {
            // GIVEN
            MockedProjectService.BuildProjectAsync("RunnableProject")
                .ThrowsAsync<NonWipProjectException>();

            // WHEN
            var response = await client.PostAsync("/projects/RunnableProject/build", null);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenProject_WhenSavingProjectDefinitions_ThenJsonDefinitionsAreReturned()
        {
            // GIVEN
            ProjectModel projectToSave = new("Project", "Project", 1, "Description", true);
            StreamWriter stream = new(new MemoryStream());
            await stream.WriteAsync(projectDefinitionsJson);
            await stream.FlushAsync();
            stream.BaseStream.Position = 0;
            MockedProjectService.GetProjectDefinitionsAsync(projectToSave.Id)
                .Returns(Task.FromResult<StreamReader?>(new StreamReader(stream.BaseStream)));
            // WHEN
            var response = await client.GetAsync($"/projects/{projectToSave.Id}/definitions");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            string definitionsJson = await response.Content.ReadAsStringAsync();
            definitionsJson.ShouldNotBeNull();
            definitionsJson = definitionsJson.Replace("\"{", "{").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\\"", "\"").Replace("}\"", "}");
            definitionsJson
                .ShouldBe(projectDefinitionsJson);
            await stream.DisposeAsync();
        }

        [Fact]
        public async Task WhenSavingUnknownProjectDefinitions_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedProjectService.GetProjectDefinitionsAsync("UnknownProject")
                .ThrowsAsync<UnknownEntityException>();
            // WHEN
            var response = await client.GetAsync($"/projects/UnknownProject/definitions");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenProjectDefinitionJson_WhenCreatingProjectFromDefinitions_ThenProjectIsCreated()
        {
            // GIVEN
            ProjectModel expectedProject = new("TestProject", "TestProject", 11, null, true);
            MockedProjectService.CreateProjectFromJsonDefinition(Arg.Any<StreamReader>())
                .Returns(expectedProject);

            // WHEN
            var response = await client.PostAsync("/projects/load-from-definitions",
                BuildJsonStreamContent(projectDefinitionsJson));

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            var createdProject = await response.Content.ReadFromJsonAsync<ProjectModel>();
            createdProject.ShouldBeEquivalentTo(expectedProject);
        }

        [Fact]
        public async Task GivenRunnableProject_WhenSavingProjectInstances_ThenJsonInstancesAreReturned()
        {
            // GIVEN
            ProjectModel projectToSave = new("Project", "Project", 1, "Description", true);
            StreamWriter stream = new(new MemoryStream());
            await stream.WriteAsync(projectInstancesJson);
            await stream.FlushAsync();
            stream.BaseStream.Position = 0;
            MockedProjectService.GetProjectInstancesAsync(projectToSave.Id)
                .Returns(Task.FromResult<StreamReader?>(new StreamReader(stream.BaseStream)));
            // WHEN
            var response = await client.GetAsync($"/projects/{projectToSave.Id}/instances");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            string instancesJson = await response.Content.ReadAsStringAsync();
            instancesJson.ShouldNotBeNull();
            instancesJson = instancesJson.Replace("\"{", "{").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\\"", "\"").Replace("}\"", "}");
            instancesJson
                .ShouldBe(projectInstancesJson);
            await stream.DisposeAsync();
        }

        [Fact]
        public async Task WhenSavingUnknownProjectInstances_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedProjectService.GetProjectInstancesAsync("UnknownProject")
                .ThrowsAsync<UnknownEntityException>();
            // WHEN
            var response = await client.GetAsync($"/projects/UnknownProject/instances");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenSavingNonRunnableProjectInstances_ThenBadRequestIsReturned()
        {
            // GIVEN
            MockedProjectService.GetProjectInstancesAsync("WipProject")
                .ThrowsAsync<NonRunnableProjectException>();
            // WHEN
            var response = await client.GetAsync($"/projects/WipProject/instances");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenRunnableProject_WhenLoadingProjectInstances_ThenJsonInstancesAreCreated()
        {
            // GIVEN

            // WHEN
            var response = await client.PostAsync("/projects/TestProject/instances",
                BuildJsonStreamContent(createProjectInstancesJson));

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task WhenLoadingUnknownProjectInstances_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedProjectService.When(fake => fake.LoadProjectInstances("UnknownProject", Arg.Any<StreamReader>()))
                .Do(call => { throw new UnknownEntityException(); });

            // WHEN
            var response = await client.PostAsync("/projects/UnknownProject/instances",
                BuildJsonStreamContent(createProjectInstancesJson));

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenLoadingNonRunnableProjectInstances_ThenBadRequestIsReturned()
        {
            // GIVEN
            MockedProjectService.When(fake => fake.LoadProjectInstances("WipProject", Arg.Any<StreamReader>()))
                .Do(call => { throw new NonRunnableProjectException(); });

            // WHEN

            var response = await client.PostAsync("/projects/WipProject/instances",
                BuildJsonStreamContent(createProjectInstancesJson));

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        private static JsonContent BuildJsonStreamContent(string json)
        {
            JsonContent jsonContent = JsonContent.Create(Encoding.UTF8.GetBytes(json));
            return jsonContent;
            //StreamContent content = new(new MemoryStream(Encoding.UTF8.GetBytes(json)));
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json", "charset=UTF-8");
            //return content;
        }

        [Fact]
        public async Task GivenProject_WhenUnloadingProject_ThenProjectIsUnloaded()
        {
            // GIVEN
            MockedProjectService.When(fake => fake.UnloadProject("UnknownProject"))
                .Do(call => { throw new UnknownEntityException(); });

            // WHEN
            var response = await client.DeleteAsync("/projects/UnknownProject");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

    }
}
