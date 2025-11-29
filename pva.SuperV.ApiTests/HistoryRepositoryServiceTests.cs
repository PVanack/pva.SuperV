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
            historyRepositoryService = new(LoggerFactory);
            runnableProject = CreateRunnableProject();
            wipProject = CreateWipProject(NullHistoryStorageEngine.Prefix);
        }

        [Fact]
        public async Task GetHistoryRepositories_ShouldReturnListOfHistoryRepositories()
        {
            // Act
            List<HistoryRepositoryModel> result = await historyRepositoryService.GetHistoryRepositoriesAsync(runnableProject.GetId());

            // Assert
            result.Count.ShouldBe(runnableProject.HistoryRepositories.Count);
        }

        [Fact]
        public async Task GetHistoryRepositoryByName_ShouldReturnHistoryRepository()
        {
            HistoryRepositoryModel expectedHistoryRepository = new(HistoryRepositoryName);
            // Act
            HistoryRepositoryModel result = await historyRepositoryService.GetHistoryRepositoryAsync(runnableProject.GetId(), expectedHistoryRepository.Name);

            // Assert
            result.ShouldBeEquivalentTo(expectedHistoryRepository);
        }

        [Fact]
        public async Task CreateHistoryRepository_ShouldCreateHistoryRepository()
        {
            HistoryRepositoryModel expectedHistoryRepository = new($"{HistoryRepositoryName}Test");
            // Act
            HistoryRepositoryModel result = await historyRepositoryService.CreateHistoryRepositoryAsync(wipProject.GetId(), expectedHistoryRepository);

            // Assert
            result.ShouldBeEquivalentTo(expectedHistoryRepository);
            wipProject.HistoryRepositories.ShouldContainKey(expectedHistoryRepository.Name);
        }

        [Fact]
        public async Task DeleteHistoryRepository_ShouldDeleteHistoryRepository()
        {
            HistoryRepositoryModel expectedHistoryRepository = new(HistoryRepositoryName);
            IFieldDefinition valueField = wipProject.GetClass(ClassName).GetField(ValueFieldName);
            List<IHistorizationProcessing> historizationProcessings = [.. valueField.ValuePostChangeProcessings.OfType<IHistorizationProcessing>()];
            historizationProcessings.ForEach(historizationProcessing => valueField.ValuePostChangeProcessings.Remove(historizationProcessing));
            IFieldDefinition intFieldWithFormatField = wipProject.GetClass(AllFieldsClassName).GetField("IntFieldWithFormat");
            historizationProcessings = [.. intFieldWithFormatField.ValuePostChangeProcessings.OfType<IHistorizationProcessing>()];
            historizationProcessings.ForEach(historizationProcessing => intFieldWithFormatField.ValuePostChangeProcessings.Remove(historizationProcessing));

            // Act
            await historyRepositoryService.DeleteHistoryRepositoryAsync(wipProject.GetId(), expectedHistoryRepository.Name);

            // Assert
            wipProject.HistoryRepositories.ShouldNotContainKey(expectedHistoryRepository.Name);
        }

        [Fact]
        public async Task DeleteInUseHistoryRepository_ThrowsException()
        {
            HistoryRepositoryModel expectedHistoryRepository = new(HistoryRepositoryName);

            // Act
            await Assert.ThrowsAsync<EntityInUseException>(async () => await historyRepositoryService.DeleteHistoryRepositoryAsync(wipProject.GetId(), expectedHistoryRepository.Name));
        }
    }
}
