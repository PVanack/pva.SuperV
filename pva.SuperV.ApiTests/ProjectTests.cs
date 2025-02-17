using NSubstitute;
using NSubstitute.ExceptionExtensions;
using pva.SuperV.Api;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using Shouldly;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace pva.SuperV.ApiTests
{
    public class ProjectTests
    {
        public class ConsoleWriter(ITestOutputHelper output) : StringWriter
        {
            public override void WriteLine(string? value) => output.WriteLine(value);
        }

        private readonly TestProjectApplication application;
        private readonly HttpClient client;
        private IProjectService MockedProjectService { get => application.MockedProjectService!; }

        public ProjectTests(ITestOutputHelper output)
        {
            application = new();
            client = application.CreateClient();
            Console.SetOut(new ConsoleWriter(output));
        }

        [Fact]
        public async Task GivenExistingProjects_WhenGettingProjects_ThenProjectsAreReturned()
        {
            // GIVEN
            List<ProjectModel> expectedProjects = [new ProjectModel("a-1", "a", 1, "Descr", false)];
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
        public async Task GivenExistingProjects_WhenGettingExistingProject_ThenProjectIsReturned()
        {
            // GIVEN
            ProjectModel expectedProject = new("a-1", "a", 1, "Descr", false);
            MockedProjectService.GetProject("a")
                .Returns(expectedProject);

            // WHEN
            var response = await client.GetAsync("/projects/a");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            ProjectModel? project = await response.Content.ReadFromJsonAsync<ProjectModel>();
            project.ShouldBeEquivalentTo(expectedProject);
        }

        [Fact]
        public async Task GivenExistingProjects_WhenGettingNonExistingProject_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedProjectService.GetProject("a")
                .Throws(new UnknownEntityException("Project", "a"));

            // WHEN
            var response = await client.GetAsync("/projects/a");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
            var message = await response.Content.ReadAsStringAsync();
            message.ShouldBe("\"Project a doesn't exist\"");
        }

        [Fact]
        public async Task GivenNoProjects_WhenCreatingNewProject_ThenProjectIsCreated()
        {
            // GIVEN
            CreateProjectRequest createProjectRequest = new("a", "Description");
            ProjectModel expectedCreatedProject = new("1", "a", 1, "descriptioon", false);
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
        public async Task GivenWipProject_WhenBuildigProjectFromIt_ThenRunnableProjectIsBuilt()
        {
            // GIVEN
            ProjectModel expectedRunnableProject = new("a-2", "a", 2, "descriptioon", true);
            MockedProjectService.BuildProject("a-1")
                .Returns(expectedRunnableProject);

            // WHEN
            var response = await client.PostAsync("/projects/build/a-1", null);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            var createdProject = await response.Content.ReadFromJsonAsync<ProjectModel>();
            createdProject.ShouldBeEquivalentTo(expectedRunnableProject);
        }

        [Fact]
        public async Task GivenRunnableProject_WhenBuildigProjectFromIt_ThenExceptionIsThrown()
        {
            // GIVEN
            MockedProjectService.BuildProject("a-1")
                .Throws(new NonWipProjectException("a-1"));

            // WHEN
            var response = await client.PostAsync("/projects/build/a-1", null);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
            var message = await response.Content.ReadAsStringAsync();
            message.ShouldBe("\"Project with ID a-1 is not a WIP project\"");
        }
    }
}
