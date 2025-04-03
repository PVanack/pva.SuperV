using NSubstitute;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.HistoryStorage;
using Shouldly;

namespace pva.SuperV.EngineTests;

public class HistoryRepositoryTests
{
    [Theory]
    [InlineData("AS.0")]
    [InlineData("0AS")]
    [InlineData("AS-0")]
    public void GivenInvalidHistoryRepositoryName_WhenCreatingRepository_ThenInvalidHistoryRepositoryNameExceptionIsThrown(string invalidHistoryRepositoryName)
    {
        // WHEN/THEN
        Assert.Throws<InvalidIdentifierNameException>(() => new HistoryRepository(invalidHistoryRepositoryName));
    }

    [Fact]
    public void GivenProjectWithHistoryRepository_WhenAddingRepository_ThenRepositoryIsAdded()
    {
        // GIVEN
        var historyStorageEngineMock = Substitute.For<IHistoryStorageEngine>();
        WipProject wipProject = Project.CreateProject("TestProject");
        wipProject.HistoryStorageEngine = historyStorageEngineMock;

        // WHEN
        HistoryRepository historyRepository = new("TestRepository");
        wipProject.AddHistoryRepository(historyRepository);

        // THEN
        wipProject.HistoryRepositories.ShouldContainKey("TestRepository");
    }

    [Fact]
    public void GivenProjectWithHistoryRepository_WhenAddingRepositoryWithSameName_ThenExceptionIsThrown()
    {
        // GIVEN
        var historyStorageEngineMock = Substitute.For<IHistoryStorageEngine>();
        WipProject wipProject = Project.CreateProject("TestProject");
        wipProject.HistoryStorageEngine = historyStorageEngineMock;
        HistoryRepository historyRepository = new("TestRepository");
        wipProject.AddHistoryRepository(historyRepository);

        // WHEN
        HistoryRepository historyRepositoryOther = new("TestRepository");
        Assert.Throws<EntityAlreadyExistException>(() => wipProject.AddHistoryRepository(historyRepositoryOther));
    }

    [Fact]
    public void GivenProjectWithHistoryRepository_WhenUpdatingRepository_ThenRepositoryIsUpdated()
    {
        // GIVEN
        var historyStorageEngineMock = Substitute.For<IHistoryStorageEngine>();
        WipProject wipProject = Project.CreateProject("TestProject");
        wipProject.HistoryStorageEngine = historyStorageEngineMock;
        HistoryRepository historyRepository = new("TestRepository");
        wipProject.AddHistoryRepository(historyRepository);

        // WHEN
        historyRepository = new("TestRepository");
        wipProject.UpdateHistoryRepository(historyRepository.Name, historyRepository);

        // THEN
        wipProject.HistoryRepositories.ShouldContainKey("TestRepository");
        wipProject.HistoryRepositories[historyRepository.Name].ShouldBe(historyRepository);
    }

    [Fact]
    public void GivenProjectWithHistoryRepository_WhenUpdatingRepositoryWithUnknownName_ThenExceptionIsThrown()
    {
        // GIVEN
        var historyStorageEngineMock = Substitute.For<IHistoryStorageEngine>();
        WipProject wipProject = Project.CreateProject("TestProject");
        wipProject.HistoryStorageEngine = historyStorageEngineMock;

        // WHEN/THEN
        HistoryRepository historyRepository = new("TestRepository");
        Assert.Throws<UnknownEntityException>(() => wipProject.UpdateHistoryRepository(historyRepository.Name, historyRepository));
    }

    [Fact]
    public void GivenProjectWithHistoryRepository_WhenRemovingHistoryRepository_ThenHistoryRepositoryIsRemoved()
    {
        // GIVEN
        var historyStorageEngineMock = Substitute.For<IHistoryStorageEngine>();
        WipProject wipProject = Project.CreateProject("TestProject");
        wipProject.HistoryStorageEngine = historyStorageEngineMock;
        HistoryRepository historyRepository = new("TestRepository");
        wipProject.AddHistoryRepository(historyRepository);

        // WHEN
        wipProject.RemoveHistoryRepository("TestRepository");

        // THEN
        wipProject.HistoryRepositories.ShouldBeEmpty();
    }

    [Fact]
    public void GivenProjectWithHistoryStorageEngine_WhenAddingRepositoryAndBuldingProject_ThenHistoryRepositoryIsCreated()
    {
        // GIVEN
        var historyStorageEngineMock = Substitute.For<IHistoryStorageEngine>();
        WipProject wipProject = Project.CreateProject("TestProject");
        wipProject.HistoryStorageEngine = historyStorageEngineMock;

        // WHEN
        HistoryRepository historyRepository = new("TestRepository");
        wipProject.AddHistoryRepository(historyRepository);
        _ = Project.BuildAsync(wipProject);

        // THEN
        historyStorageEngineMock.Received(1).UpsertRepository(wipProject.Name!, historyRepository);
    }

    [Fact]
    public async Task GivenProjectWithoutHistoryStorageEngine_WhenAddingRepositoryAndBuldingProject_ThenExceptionIsThrown()
    {
        // GIVEN
        WipProject wipProject = Project.CreateProject("TestProject");

        // WHEN/THEN
        HistoryRepository historyRepository = new("TestRepository");
        wipProject.AddHistoryRepository(historyRepository);
        await Assert.ThrowsAsync<NoHistoryStorageEngineException>(async () => _ = await Project.BuildAsync(wipProject));
    }
}
