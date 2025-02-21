using pva.SuperV.Api;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.Projects;
using Shouldly;
using System.Text;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class ProjectServiceTests
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
        private readonly ProjectService _projectService;
        private readonly RunnableProject runnableProject;

        public ProjectServiceTests()
        {
            _projectService = new();
            runnableProject = ProjectHelpers.CreateRunnableProject();
        }

        [Fact]
        public void GetProjects_ShouldReturnListOfProjects()
        {
            // Act
            List<ProjectModel> result = _projectService.GetProjects();

            // Assert
            result.ShouldContain(p
                => p.Id == runnableProject.GetId() &&
                p.Runnable &&
                p.Name == ProjectHelpers.ProjectName);
        }

        [Fact]
        public void GetProject_ShouldReturnProject_WhenProjectExists()
        {
            // Act
            var result = _projectService.GetProject(runnableProject.GetId());

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(runnableProject.GetId());
        }

        [Fact]
        public void GetProject_ShouldThrowUnknownEntityException_WhenProjectDoesNotExist()
        {
            // Act & Assert
            Assert.Throws<UnknownEntityException>(()
                => _projectService.GetProject("NonExistentId"));
        }

        [Fact]
        public void CreateProject_ShouldReturnCreatedProject()
        {
            // Arrange
            CreateProjectRequest createProjectRequest = new(Name: "NewProject", Description: "Description");

            // Act
            var result = _projectService.CreateProject(createProjectRequest);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBeNull();
            result.Name.ShouldBe("NewProject");
            result.Runnable.ShouldBeFalse();
        }

        [Fact]
        public void BuildProject_ShouldReturnRunnableProject_WhenProjectIsWip()
        {
            // Act
            WipProject wipProject = Project.CreateProject("WipProject1");
            var result = _projectService.BuildProject(wipProject.GetId());

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBeNull();
            result.Name.ShouldBe("WipProject1");
            result.Runnable.ShouldBeTrue();
        }

        [Fact]
        public void BuildProject_ShouldThrowNonWipProjectException_WhenProjectIsNotWip()
        {
            WipProject wipProject = Project.CreateProject("WipProject2");
            RunnableProject runProject = Project.Build(wipProject);
            // Act & Assert
            Assert.Throws<NonWipProjectException>(()
                => _projectService.BuildProject(runProject.GetId()));
        }

        [Fact]
        public void SaveProjectDefinitionToJson_ShouldReturnProjectDefinitionsAsJson()
        {
            // Act
            var result = _projectService.GetProjectDefinitions(runnableProject.GetId());

            // Assert
            result.ShouldNotBeNull();
         }


        [Fact]
        public void CreateProjectFromDefinitionJson_ShouldReturnRunnableProject()
        {
            // Act
            using StreamReader definitionsStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(projectDefinitionsJson)));
            var result = _projectService.CreateProjectFromJsonDefinition(definitionsStream);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBeNull();
            result.Name.ShouldBe("TestProject");
            result.Runnable.ShouldBeTrue();
        }

        [Fact]
        public void UnloadProject_ShouldRemoveProject()
        {
            // Act
            WipProject wipProject = Project.CreateProject("WipProject1");
            _projectService.UnloadProject(wipProject.GetId());

            // Assert
            Assert.Throws<UnknownEntityException>(()
                => _projectService.GetProject(wipProject.GetId()));
        }

    }
}