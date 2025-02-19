using NSubstitute;
using NSubstitute.ExceptionExtensions;
using pva.SuperV.Api;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using Shouldly;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace pva.SuperV.ApiTests
{
    public class ProjectEndpointsTests
    {
        public class ConsoleWriter(ITestOutputHelper output) : StringWriter
        {
            public override void WriteLine(string? value) => output.WriteLine(value);
        }

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
        public async Task GivenExistingProjects_WhenGettingExistingProject_ThenProjectIsReturned()
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
        public async Task GivenExistingProjects_WhenGettingNonExistingProject_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedProjectService.GetProject("UnknownProject")
                .Throws(new UnknownEntityException("Project", "UnknownProject"));

            // WHEN
            var response = await client.GetAsync("/projects/UnknownProject");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
            var message = await response.Content.ReadAsStringAsync();
            message.ShouldBe("\"Project UnknownProject doesn't exist\"");
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
        public async Task GivenWipProject_WhenBuildigProjectFromIt_ThenRunnableProjectIsBuilt()
        {
            // GIVEN
            ProjectModel expectedRunnableProject = new("Project-Wip", "a", 2, "description", true);
            MockedProjectService.BuildProject("Project-Wip")
                .Returns(expectedRunnableProject);

            // WHEN
            var response = await client.PostAsync("/projects/build/Project-Wip", null);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            var createdProject = await response.Content.ReadFromJsonAsync<ProjectModel>();
            createdProject.ShouldBeEquivalentTo(expectedRunnableProject);
        }

        [Fact]
        public async Task GivenRunnableProject_WhenBuildigProjectFromIt_ThenExceptionIsThrown()
        {
            // GIVEN
            MockedProjectService.BuildProject("Project")
                .Throws(new NonWipProjectException("Project"));

            // WHEN
            var response = await client.PostAsync("/projects/build/Project", null);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
            var message = await response.Content.ReadAsStringAsync();
            message.ShouldBe("\"Project with ID Project is not a WIP project\"");
        }
    }
}
