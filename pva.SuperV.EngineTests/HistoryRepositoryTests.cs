using NSubstitute;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
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
        Assert.Throws<InvalidHistoryRepositoryNameException>(() => new HistoryRepository(invalidHistoryRepositoryName));
    }


    [Fact]
    public void GivenProjectWithHistoryRepository_WhenAddingRepositoryWithSameName_ThenExceptionIsThrown()
    {
        // GIVEN
        var historyStorageEngineMock = Substitute.For<IHistoryStorageEngine>();
        WipProject wipProject = Project.CreateProject(ProjectHelpers.ProjectName);
        wipProject.HistoryStorageEngine = historyStorageEngineMock;
        HistoryRepository historyRepository = new(ProjectHelpers.HistoryRepositoryName);
        wipProject.AddHistoryRepository(historyRepository);

        // WHEN
        HistoryRepository historyRepositoryOther = new(ProjectHelpers.HistoryRepositoryName);
        Assert.Throws<HistoryRepositoryAlreadyExistException>(() => wipProject.AddHistoryRepository(historyRepositoryOther));
    }

    [Fact]
    public void GivenProjectWithHistoryRepository_WhenRemovingHistoryRepository_ThenHistoryRepositoryIsRemoved()
    {
        // GIVEN
        var historyStorageEngineMock = Substitute.For<IHistoryStorageEngine>();
        WipProject wipProject = Project.CreateProject(ProjectHelpers.ProjectName);
        wipProject.HistoryStorageEngine = historyStorageEngineMock;
        HistoryRepository historyRepository = new(ProjectHelpers.HistoryRepositoryName);
        wipProject.AddHistoryRepository(historyRepository);

        // WHEN
        wipProject.RemoveHistoryRepository(ProjectHelpers.HistoryRepositoryName);

        // THEN
        wipProject.HistoryRepositories.ShouldBeEmpty();
    }

    [Fact]
    public void GivenProjectWithHistoryStorageEngine_WhenAddingRepositoryAndBuldingProject_ThenHistoryRepositoryIsCreated()
    {
        // GIVEN
        var historyStorageEngineMock = Substitute.For<IHistoryStorageEngine>();
        WipProject wipProject = Project.CreateProject(ProjectHelpers.ProjectName);
        wipProject.HistoryStorageEngine = historyStorageEngineMock;

        // WHEN
        HistoryRepository historyRepository = new(ProjectHelpers.HistoryRepositoryName);
        wipProject.AddHistoryRepository(historyRepository);
        historyStorageEngineMock.RepositoryExists(ProjectHelpers.HistoryRepositoryName)
            .Returns(false);
        _ = ProjectBuilder.Build(wipProject);

        // THEN
        historyStorageEngineMock.Received().CreateRepository(historyRepository);
    }

    [Fact]
    public void GivenProjectWithHistoryStorageEngine_WhenUpdatingRepositoryAndBuldingProject_ThenHistoryRepositoryIsUpdated()
    {
        // GIVEN
        var historyStorageEngineMock = Substitute.For<IHistoryStorageEngine>();
        WipProject wipProject = Project.CreateProject(ProjectHelpers.ProjectName);
        wipProject.HistoryStorageEngine = historyStorageEngineMock;

        // WHEN
        HistoryRepository historyRepository = new(ProjectHelpers.HistoryRepositoryName);
        wipProject.AddHistoryRepository(historyRepository);
        historyStorageEngineMock.RepositoryExists(ProjectHelpers.HistoryRepositoryName)
            .Returns(true);
        _ = ProjectBuilder.Build(wipProject);

        // THEN
        historyStorageEngineMock.Received().UpdateRepository(historyRepository);
    }

    [Fact]
    public void GivenProjectWithoutHistoryStorageEngine_WhenAddingRepositoryAndBuldingProject_ThenExceptionIsThrown()
    {
        // GIVEN
        WipProject wipProject = Project.CreateProject(ProjectHelpers.ProjectName);

        // WHEN/THEN
        HistoryRepository historyRepository = new(ProjectHelpers.HistoryRepositoryName);
        wipProject.AddHistoryRepository(historyRepository);
        Assert.Throws<NoHistoryStorageEngineException>(() => _ = ProjectBuilder.Build(wipProject));
    }
}
