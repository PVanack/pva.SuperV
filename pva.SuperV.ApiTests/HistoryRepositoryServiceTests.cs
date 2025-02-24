using pva.SuperV.Api;
using pva.SuperV.Engine;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.HistoryRepositories;
using pva.SuperV.Model.Projects;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class HistoryRepositoryServiceTests
    {
        private readonly HistoryRepositoryService historyRepositoryService;
        private readonly RunnableProject runnableProject;
        private readonly WipProject wipProject;

        public HistoryRepositoryServiceTests()
        {
            historyRepositoryService = new();
            runnableProject = ProjectHelpers.CreateRunnableProject();
            wipProject = ProjectHelpers.CreateWipProject(NullHistoryStorageEngine.Prefix);
        }

        [Fact]
        public void GetHistoryRepositories_ShouldReturnListOfHistoryRepositories()
        {
            // Act
            List<HistoryRepositoryModel> result = historyRepositoryService.GetHistoryRepositories(runnableProject.GetId());

            // Assert
            result.Count.ShouldBe(runnableProject.HistoryRepositories.Count);
        }

        [Fact]
        public void GetHistoryRepositoryByName_ShouldReturnHistoryRepository()
        {
            HistoryRepositoryModel expectedHistoryRepository = new(ProjectHelpers.HistoryRepositoryName);
            // Act
            HistoryRepositoryModel result = historyRepositoryService.GetHistoryRepository(runnableProject.GetId(), expectedHistoryRepository.Name);

            // Assert
            result.ShouldBeEquivalentTo(expectedHistoryRepository);
        }

        [Fact]
        public void CreateHistoryRepository_ShouldCreateHistoryRepository()
        {
            HistoryRepositoryModel expectedHistoryRepository = new($"{ProjectHelpers.HistoryRepositoryName}Test");
            // Act
            HistoryRepositoryModel result = historyRepositoryService.CreateHistoryRepository(wipProject.GetId(), expectedHistoryRepository);

            // Assert
            result.ShouldBeEquivalentTo(expectedHistoryRepository);
            wipProject.HistoryRepositories.ShouldContainKey(expectedHistoryRepository.Name);
        }

        [Fact]
        public void DeleteHistoryRepository_ShouldDeleteHistoryRepository()
        {
            HistoryRepositoryModel expectedHistoryRepository = new($"{ProjectHelpers.HistoryRepositoryName}");
            // Act
            historyRepositoryService.DeleteHistoryRepository(wipProject.GetId(), expectedHistoryRepository.Name);

            // Assert
            wipProject.HistoryRepositories.ShouldNotContainKey(expectedHistoryRepository.Name);
        }
    }
}
