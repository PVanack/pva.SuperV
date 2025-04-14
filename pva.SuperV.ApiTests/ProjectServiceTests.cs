using pva.SuperV.Api.Exceptions;
using pva.SuperV.Api.Services.Projects;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.EngineTests;
using pva.SuperV.Model;
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
        public void SearchProjectsPaged_ShouldReturnPageOfProjects()
        {
            CreateDummyWipProjects();

            // Act
            ProjectPagedSearchRequest search = new(1, 5, null, null);
            PagedSearchResult<ProjectModel> page1Result = projectService.SearchProjects(search);
            search = search with { PageNumber = 2, PageSize = 10 };
            PagedSearchResult<ProjectModel> page2Result = projectService.SearchProjects(search);
            search = search with { PageNumber = 3 };
            PagedSearchResult<ProjectModel> page3Result = projectService.SearchProjects(search);
            search = search with { PageNumber = 4 };
            PagedSearchResult<ProjectModel> page4Result = projectService.SearchProjects(search);

            // Assert
            List<ProjectModel> expectedProjects = [.. Project.Projects.Select(entry => ProjectMapper.ToDto(entry.Value))];

            page1Result.ShouldNotBeNull();
            page1Result.PageNumber.ShouldBe(1);
            page1Result.PageSize.ShouldBe(5);
            page1Result.Count.ShouldBe(Project.Projects.Count);
            page1Result.Result.ShouldBeEquivalentTo(expectedProjects.Take(5).ToList());

            page2Result.ShouldNotBeNull();
            page2Result.PageNumber.ShouldBe(2);
            page2Result.PageSize.ShouldBe(10);
            page2Result.Count.ShouldBe(Project.Projects.Count);
            page2Result.Result.ShouldBeEquivalentTo(expectedProjects.Skip(10).Take(10).ToList());

            page3Result.ShouldNotBeNull();
            page3Result.PageNumber.ShouldBe(3);
            page3Result.PageSize.ShouldBe(10);
            page3Result.Count.ShouldBe(Project.Projects.Count);
            page3Result.Result.ShouldBeEquivalentTo(expectedProjects.Skip(20).Take(10).ToList());

            page4Result.ShouldNotBeNull();
            page4Result.PageNumber.ShouldBe(4);
            page4Result.PageSize.ShouldBe(10);
            page4Result.Count.ShouldBe(Project.Projects.Count);
            page4Result.Result.ShouldBeEmpty();
        }

        [Fact]
        public void SearchProjectsSortedByNameAsc_ShouldReturnPageOfProjectsSorted()
        {
            CreateDummyWipProjects();

            // Act
            ProjectPagedSearchRequest search = new(1, 5, null, "name");
            PagedSearchResult<ProjectModel> pagedResult = projectService.SearchProjects(search);

            // Assert
            List<ProjectModel> expectedProjects = [.. Project.Projects.Values.Select(project => ProjectMapper.ToDto(project))];
            expectedProjects.Sort(new Comparison<ProjectModel>((a, b) => a.Name.CompareTo(b.Name)));

            pagedResult.ShouldNotBeNull();
            pagedResult.PageNumber.ShouldBe(1);
            pagedResult.PageSize.ShouldBe(5);
            pagedResult.Count.ShouldBe(Project.Projects.Count);
            pagedResult.Result.ShouldBeEquivalentTo(expectedProjects.Take(5).ToList());
        }

        [Fact]
        public void SearchProjectsSortByNameDesc_ShouldReturnPageOfProjectsSorted()
        {
            CreateDummyWipProjects();

            // Act
            ProjectPagedSearchRequest search = new(1, 5, null, "-name");
            PagedSearchResult<ProjectModel> pagedResult = projectService.SearchProjects(search);

            // Assert
            List<ProjectModel> expectedProjects = [.. Project.Projects.Values.Select(project => ProjectMapper.ToDto(project))];
            expectedProjects.Sort(new Comparison<ProjectModel>((a, b) => a.Name.CompareTo(b.Name)));
            expectedProjects.Reverse();

            pagedResult.ShouldNotBeNull();
            pagedResult.PageNumber.ShouldBe(1);
            pagedResult.PageSize.ShouldBe(5);
            pagedResult.Count.ShouldBe(Project.Projects.Count);
            pagedResult.Result.ShouldBeEquivalentTo(expectedProjects.Take(5).ToList());
        }

        [Fact]
        public void SearchProjectsWithInvalidSortOption_ShouldThrowException()
        {
            // Act
            ProjectPagedSearchRequest search = new(1, 5, null, "-UnknownOption");
            Assert.Throws<InvalidSortOptionException>(() => projectService.SearchProjects(search));
        }

        [Fact]
        public void SearchProjectsByName_ShouldReturnPageOfProjects()
        {
            CreateDummyWipProjects();

            // Act
            ProjectPagedSearchRequest search = new(1, 5, "DummyProject1", null);
            PagedSearchResult<ProjectModel> pagedResult = projectService.SearchProjects(search);

            // Assert
            List<ProjectModel> expectedProjects = [.. Project.Projects.Values
                .Where(project=> project.Name!.Contains("DummyProject1"))
                .Select(project => ProjectMapper.ToDto(project))];

            pagedResult.ShouldNotBeNull();
            pagedResult.PageNumber.ShouldBe(1);
            pagedResult.PageSize.ShouldBe(5);
            pagedResult.Count.ShouldBe(Project.Projects.Count);
            pagedResult.Result.ShouldBeEquivalentTo(expectedProjects.Take(5).ToList());
        }

        private static void CreateDummyWipProjects()
        {
            for (int i = 0; i < 10; i++)
            {
                Project.CreateProject($"DummyProject{i + 1}");
            }
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
        public void UpdateProject_ShouldReturnUpdatedProject()
        {
            // Arrange
            ProjectModel createdProject = projectService.CreateProject(new CreateProjectRequest(Name: "NewProject", Description: "Description"));

            // Act
            UpdateProjectRequest updateProjectRequest = new("Description2", NullHistoryStorageEngine.Prefix);
            ProjectModel result = projectService.UpdateProject(createdProject.Id, updateProjectRequest);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBeNull();
            result.Description.ShouldBe(updateProjectRequest.Description);
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