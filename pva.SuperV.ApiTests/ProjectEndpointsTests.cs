﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using pva.SuperV.Api;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Projects;
using Shouldly;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Xml.Linq;
using TDengine.Driver.Client.Websocket;
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
            var response = await client.PostAsync("/projects/Project-Wip/build", null);

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
            var response = await client.PostAsync("/projects/Project/build", null);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
            var message = await response.Content.ReadAsStringAsync();
            message.ShouldBe("\"Project with ID Project is not a WIP project\"");
        }

        [Fact]
        public async Task GivenProject_WhenSavingProjectDefinitions_ThenJsonDefinitionsAreReturned()
        {
            // GIVEN
            ProjectModel projectToSave = new("Project", "Project", 1, "Description", true);
            MockedProjectService.GetProjectDefinitions(projectToSave.Id)
                .Returns(projectDefinitionsJson);
            // WHEN
            var response = await client.GetAsync($"/projects/{projectToSave.Id}/definitions");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            string definitionsJson = await response.Content.ReadAsStringAsync();
            definitionsJson.ShouldNotBeNull()
                .ShouldBe(projectDefinitionsJson);
        }


        [Fact]
        public async Task GivenProjectDefinitionJson_WhenCreatingProjectFromDefinitions_ThenProjectIsCreated()
        {
            // GIVEN
            ProjectModel expectedProject = new("TestProject", "TestProject", 11, null, true);
            MockedProjectService.CreateProjectFromJsonDefinition(Arg.Any<StreamReader>())
                .Returns(expectedProject);

            // WHEN
            using var form = new MultipartFormDataContent();
            var jsonContent = new ByteArrayContent(Encoding.UTF8.GetBytes(projectDefinitionsJson));
            jsonContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
            form.Add(jsonContent);

            var response = await client.PostAsync("/projects/load-from-definitions", form);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            var createdProject = await response.Content.ReadFromJsonAsync<ProjectModel>();
            createdProject.ShouldBeEquivalentTo(expectedProject);
        }

        [Fact]
        public async Task GivenProject_WhenUnloadingProject_ThenProjectIsUnloaded()
        {
            // GIVEN

            // WHEN
            var response = await client.DeleteAsync("/projects/Project-Wip");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        }
    }
}
