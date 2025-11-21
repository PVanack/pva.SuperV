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
        public async Task SearchProjectsPaged_ShouldReturnPageOfProjects()
        {
            CreateDummyWipProjects();

            // Act
            ProjectPagedSearchRequest search = new(1, 5, null, null);
            PagedSearchResult<ProjectModel> page1Result = await projectService.SearchProjectsAsync(search);
            search = search with { PageNumber = 2, PageSize = 10 };
            PagedSearchResult<ProjectModel> page2Result = await projectService.SearchProjectsAsync(search);
            search = search with { PageNumber = 3 };
            PagedSearchResult<ProjectModel> page3Result = await projectService.SearchProjectsAsync(search);
            search = search with { PageNumber = 4 };
            PagedSearchResult<ProjectModel> page4Result = await projectService.SearchProjectsAsync(search);

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
        public async Task SearchProjectsSortedByNameAsc_ShouldReturnPageOfProjectsSorted()
        {
            CreateDummyWipProjects();

            // Act
            ProjectPagedSearchRequest search = new(1, 5, null, "name");
            PagedSearchResult<ProjectModel> pagedResult = await projectService.SearchProjectsAsync(search);

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
        public async Task SearchProjectsSortByNameDesc_ShouldReturnPageOfProjectsSorted()
        {
            CreateDummyWipProjects();

            // Act
            ProjectPagedSearchRequest search = new(1, 5, null, "-name");
            PagedSearchResult<ProjectModel> pagedResult = await projectService.SearchProjectsAsync(search);

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
        public async Task SearchProjectsWithInvalidSortOption_ShouldThrowException()
        {
            // Act
            ProjectPagedSearchRequest search = new(1, 5, null, "-UnknownOption");
            await Assert.ThrowsAsync<InvalidSortOptionException>(async () => await projectService.SearchProjectsAsync(search));
        }

        [Fact]
        public async Task SearchProjectsByName_ShouldReturnPageOfProjects()
        {
            CreateDummyWipProjects();

            // Act
            ProjectPagedSearchRequest search = new(1, 5, "DummyProject1", null);
            PagedSearchResult<ProjectModel> pagedResult = await projectService.SearchProjectsAsync(search);

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
        public async Task GetProjects_ShouldReturnListOfProjects()
        {
            // Act
            List<ProjectModel> result = await projectService.GetProjectsAsync();

            // Assert
            result.ShouldContain(p
                => p.Id == runnableProject.GetId() &&
                p.Runnable &&
                p.Name == ProjectName);
        }

        [Fact]
        public async Task GetProject_ShouldReturnProject_WhenProjectExists()
        {
            // Act
            var result = await projectService.GetProjectAsync(runnableProject.GetId());

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(runnableProject.GetId());
        }

        [Fact]
        public async Task GetProject_ShouldThrowUnknownEntityException_WhenProjectDoesNotExist()
        {
            // Act & Assert
            await Assert.ThrowsAsync<UnknownEntityException>(async ()
                => await projectService.GetProjectAsync("NonExistentId"));
        }

        [Fact]
        public async Task CreateProject_ShouldReturnCreatedProject()
        {
            // Arrange
            CreateProjectRequest createProjectRequest = new(Name: "NewProject", Description: "Description");

            // Act
            var result = await projectService.CreateProjectAsync(createProjectRequest);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBeNull();
            result.Name.ShouldBe("NewProject");
            result.Runnable.ShouldBeFalse();
        }

        [Fact]
        public async Task UpdateProject_ShouldReturnUpdatedProject()
        {
            // Arrange
            ProjectModel createdProject = await projectService.CreateProjectAsync(new CreateProjectRequest(Name: "NewProject", Description: "Description"));

            // Act
            UpdateProjectRequest updateProjectRequest = new("Description2", NullHistoryStorageEngine.Prefix);
            ProjectModel result = await projectService.UpdateProjectAsync(createdProject.Id, updateProjectRequest);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBeNull();
            result.Description.ShouldBe(updateProjectRequest.Description);
            result.Runnable.ShouldBeFalse();
        }

        [Fact]
        public async Task CreateProjectFromRunnable_ShouldReturnCreatedProject()
        {
            // Arrange

            // Act
            var result = await projectService.CreateProjectFromRunnableAsync(runnableProject.GetId());

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
            var stream = await projectService.GetProjectDefinitionsAsync(runnableProject.GetId());

            // Assert
            stream.ShouldNotBeNull();
            using StreamReader streamReader = new(stream);
            string projectDefinitions = await streamReader.ReadToEndAsync(TestContext.Current.CancellationToken);
            projectDefinitions.ShouldNotBeNullOrEmpty();
        }


        [Fact]
        public async Task CreateProjectFromDefinitionJson_ShouldReturnRunnableProject()
        {
            // Act
            using StreamReader definitionsStream = new(new MemoryStream(Encoding.UTF8.GetBytes(projectDefinitionsJson)));
            var result = await projectService.CreateProjectFromJsonDefinitionAsync(definitionsStream);

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
            var stream = await projectService.GetProjectInstancesAsync(runnableProject.GetId());

            // Assert
            stream.ShouldNotBeNull();
            using StreamReader streamReader = new(stream);
            string projectInstances = await streamReader.ReadToEndAsync(TestContext.Current.CancellationToken);
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
        public async Task LoadRunnableProjectInstancesFromJson_ShouldCreateInstances()
        {
            // Act
            using StreamReader definitionsStream = new(new MemoryStream(Encoding.UTF8.GetBytes(projectInstancesJson)));
            await projectService.LoadProjectInstancesAsync(runnableProject.GetId(), definitionsStream);

            // Assert
            runnableProject.Instances.ShouldContainKey("InstanceToCreate");
        }

        [Fact]
        public async Task LoadWipProjectInstancesFromJson_ShouldThrowException()
        {
            // Act
            using StreamReader definitionsStream = new(new MemoryStream(Encoding.UTF8.GetBytes(projectInstancesJson)));
            await Assert.ThrowsAsync<NonRunnableProjectException>(async () => await projectService.LoadProjectInstancesAsync(wipProject.GetId(), definitionsStream));

            // Assert
        }

        [Fact]
        public async Task UnloadProject_ShouldRemoveProject()
        {
            // Act
            WipProject wipProject1 = Project.CreateProject("WipProject1");
            await projectService.UnloadProjectAsync(wipProject1.GetId());

            // Assert
            await Assert.ThrowsAsync<UnknownEntityException>(async ()
                => await projectService.GetProjectAsync(wipProject1.GetId()));
        }

    }
}