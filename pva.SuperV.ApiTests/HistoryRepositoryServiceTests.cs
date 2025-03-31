using pva.SuperV.Api.Services.HistoryRepositories;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Engine.Processing;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.HistoryRepositories;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class HistoryRepositoryServiceTests : SuperVTestsBase
    {
        private readonly HistoryRepositoryService historyRepositoryService;
        private readonly RunnableProject runnableProject;
        private readonly WipProject wipProject;

        public HistoryRepositoryServiceTests()
        {
            historyRepositoryService = new();
            runnableProject = CreateRunnableProject();
            wipProject = CreateWipProject(NullHistoryStorageEngine.Prefix);
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
            HistoryRepositoryModel expectedHistoryRepository = new(HistoryRepositoryName);
            // Act
            HistoryRepositoryModel result = historyRepositoryService.GetHistoryRepository(runnableProject.GetId(), expectedHistoryRepository.Name);

            // Assert
            result.ShouldBeEquivalentTo(expectedHistoryRepository);
        }

        [Fact]
        public void CreateHistoryRepository_ShouldCreateHistoryRepository()
        {
            HistoryRepositoryModel expectedHistoryRepository = new($"{HistoryRepositoryName}Test");
            // Act
            HistoryRepositoryModel result = historyRepositoryService.CreateHistoryRepository(wipProject.GetId(), expectedHistoryRepository);

            // Assert
            result.ShouldBeEquivalentTo(expectedHistoryRepository);
            wipProject.HistoryRepositories.ShouldContainKey(expectedHistoryRepository.Name);
        }

        [Fact]
        public void DeleteHistoryRepository_ShouldDeleteHistoryRepository()
        {
            HistoryRepositoryModel expectedHistoryRepository = new($"{HistoryRepositoryName}");
            IFieldDefinition valueField = wipProject.GetClass(ClassName).GetField(ValueFieldName);
            List<IHistorizationProcessing> historizationProcessings = valueField.ValuePostChangeProcessings.OfType<IHistorizationProcessing>()
                .ToList();
            historizationProcessings.ForEach(historizationProcessing => valueField.ValuePostChangeProcessings.Remove(historizationProcessing));
            IFieldDefinition intFieldWithFormatField = wipProject.GetClass(AllFieldsClassName).GetField("IntFieldWithFormat");
            historizationProcessings = intFieldWithFormatField.ValuePostChangeProcessings.OfType<IHistorizationProcessing>()
                .ToList();
            historizationProcessings.ForEach(historizationProcessing => intFieldWithFormatField.ValuePostChangeProcessings.Remove(historizationProcessing));

            // Act
            historyRepositoryService.DeleteHistoryRepository(wipProject.GetId(), expectedHistoryRepository.Name);

            // Assert
            wipProject.HistoryRepositories.ShouldNotContainKey(expectedHistoryRepository.Name);
        }

        [Fact]
        public void DeleteInUseHistoryRepository_ThrowsException()
        {
            HistoryRepositoryModel expectedHistoryRepository = new($"{HistoryRepositoryName}");

            // Act
            Assert.Throws<EntityInUseException>(() => historyRepositoryService.DeleteHistoryRepository(wipProject.GetId(), expectedHistoryRepository.Name));
        }
    }
}
