using pva.SuperV.Api;
using pva.SuperV.Api.Services.Projects;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.Projects;
using Shouldly;
using System.Text;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class ProjectServiceTests : SuperVTestsBase
    {
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
                        "Name": "InstanceToCreate",
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
        private readonly ProjectService projectService;
        private readonly RunnableProject runnableProject;
        private readonly WipProject wipProject;

        public ProjectServiceTests()
        {
            projectService = new();
            runnableProject = CreateRunnableProject();
            wipProject = CreateWipProject(null);
        }

        [Fact]
        public void GetProjects_ShouldReturnListOfProjects()
        {
            // Act
            List<ProjectModel> result = projectService.GetProjects();

            // Assert
            result.ShouldContain(p
                => p.Id == runnableProject.GetId() &&
                p.Runnable &&
                p.Name == ProjectName);
        }

        [Fact]
        public void GetProject_ShouldReturnProject_WhenProjectExists()
        {
            // Act
            var result = projectService.GetProject(runnableProject.GetId());

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(runnableProject.GetId());
        }

        [Fact]
        public void GetProject_ShouldThrowUnknownEntityException_WhenProjectDoesNotExist()
        {
            // Act & Assert
            Assert.Throws<UnknownEntityException>(()
                => projectService.GetProject("NonExistentId"));
        }

        [Fact]
        public void CreateProject_ShouldReturnCreatedProject()
        {
            // Arrange
            CreateProjectRequest createProjectRequest = new(Name: "NewProject", Description: "Description");

            // Act
            var result = projectService.CreateProject(createProjectRequest);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBeNull();
            result.Name.ShouldBe("NewProject");
            result.Runnable.ShouldBeFalse();
        }

        [Fact]
        public void CreateProjectFromRunnable_ShouldReturnCreatedProject()
        {
            // Arrange

            // Act
            var result = projectService.CreateProjectFromRunnable(runnableProject.GetId());

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe(runnableProject.Name);
            result.Version.ShouldBe(runnableProject.Version + 1);
            result.Runnable.ShouldBeFalse();
        }

        [Fact]
        public async Task BuildProject_ShouldReturnRunnableProject_WhenProjectIsWip()
        {
            // Act
            WipProject wipProject1 = Project.CreateProject("WipProject1");
            var result = await projectService.BuildProjectAsync(wipProject1.GetId());

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBeNull();
            result.Name.ShouldBe("WipProject1");
            result.Runnable.ShouldBeTrue();
        }

        [Fact]
        public async Task BuildProject_ShouldThrowNonWipProjectException_WhenProjectIsNotWip()
        {
            WipProject wipProject2 = Project.CreateProject("WipProject2");
            RunnableProject runProject = await Project.BuildAsync(wipProject2);
            // Act & Assert
            await Assert.ThrowsAsync<NonWipProjectException>(async ()
                => await projectService.BuildProjectAsync(runProject.GetId()));
        }

        [Fact]
        public async Task SaveProjectDefinitionToJson_ShouldReturnProjectDefinitionsAsJsonAsync()
        {
            // Act
            var result = await projectService.GetProjectDefinitionsAsync(runnableProject.GetId());

            // Assert
            result.ShouldNotBeNull();
            string projectDefinitions = await result.ReadToEndAsync();
            projectDefinitions.ShouldNotBeNullOrEmpty();
        }


        [Fact]
        public void CreateProjectFromDefinitionJson_ShouldReturnRunnableProject()
        {
            // Act
            using StreamReader definitionsStream = new(new MemoryStream(Encoding.UTF8.GetBytes(projectDefinitionsJson)));
            var result = projectService.CreateProjectFromJsonDefinition(definitionsStream);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBeNull();
            result.Name.ShouldBe("TestProject");
            result.Runnable.ShouldBeTrue();
        }

        [Fact]
        public async Task SaveRunnableProjectInstancesToJson_ShouldReturnProjectInstancesAsJsonAsync()
        {
            // Act
            var result = await projectService.GetProjectInstancesAsync(runnableProject.GetId());

            // Assert
            result.ShouldNotBeNull();
            string projectInstances = await result.ReadToEndAsync();
            projectInstances.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task SaveWipProjectInstancesToJson_ShouldThrowException()
        {
            // Act
            await Assert.ThrowsAsync<NonRunnableProjectException>(() => projectService.GetProjectInstancesAsync(wipProject.GetId()));

            // Assert    
        }

        [Fact]
        public void LoadRunnableProjectInstancesFromJson_ShouldCreateInstances()
        {
            // Act
            using StreamReader definitionsStream = new(new MemoryStream(Encoding.UTF8.GetBytes(projectInstancesJson)));
            projectService.LoadProjectInstances(runnableProject.GetId(), definitionsStream);

            // Assert
            runnableProject.Instances.ShouldContainKey("InstanceToCreate");
        }

        [Fact]
        public void LoadWipProjectInstancesFromJson_ShouldThrowException()
        {
            // Act
            using StreamReader definitionsStream = new(new MemoryStream(Encoding.UTF8.GetBytes(projectInstancesJson)));
            Assert.Throws<NonRunnableProjectException>(() => projectService.LoadProjectInstances(wipProject.GetId(), definitionsStream));

            // Assert
        }

        [Fact]
        public void UnloadProject_ShouldRemoveProject()
        {
            // Act
            WipProject wipProject1 = Project.CreateProject("WipProject1");
            projectService.UnloadProject(wipProject1.GetId());

            // Assert
            Assert.Throws<UnknownEntityException>(()
                => projectService.GetProject(wipProject1.GetId()));
        }

    }
}