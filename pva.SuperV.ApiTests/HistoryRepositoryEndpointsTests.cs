﻿using NSubstitute;
using NSubstitute.ExceptionExtensions;
using pva.SuperV.Api.Exceptions;
using pva.SuperV.Api.Services.HistoryRepositories;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.HistoryRepositories;
using Shouldly;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace pva.SuperV.ApiTests
{
    public class HistoryRepositoryEndpointsTests
    {
        public class ConsoleWriter(ITestOutputHelper output) : StringWriter
        {
            public override void WriteLine(string? value) => output.WriteLine(value);
        }

        private readonly TestProjectApplication application;
        private readonly HttpClient client;
        private IHistoryRepositoryService MockedHistoryRepositoryService { get => application.MockedHistoryRepositoryService!; }

        public HistoryRepositoryEndpointsTests(ITestOutputHelper output)
        {
            application = new();
            client = application.CreateClient();
            Console.SetOut(new ConsoleWriter(output));
        }

        [Fact]
        public async Task GivenProjectWithHistoryRepositories_WhenGettingHistoryRepositories_ThenHistoryRepositoriesAreReturned()
        {
            // GIVEN
            List<HistoryRepositoryModel> expectedHistoryRepositories = [new HistoryRepositoryModel("Repository1")];
            MockedHistoryRepositoryService.GetHistoryRepositories("Project")
                .Returns(expectedHistoryRepositories);
            // WHEN
            var result = await client.GetAsync("/history-repositories/Project");

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            HistoryRepositoryModel[]? historyRepositories = await result.Content.ReadFromJsonAsync<HistoryRepositoryModel[]>();
            historyRepositories.ShouldBeEquivalentTo(expectedHistoryRepositories.ToArray());
        }

        [Fact]
        public async Task WhenGettingHistoryRepositoriesOnUnknownProject_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedHistoryRepositoryService.GetHistoryRepositories("UnknownProject")
                .Throws<UnknownEntityException>();
            // WHEN
            var result = await client.GetAsync("/history-repositories/UnknownProject");

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenProjectWithHistoryRepositories_WhenGettingHistoryRepository_ThenHistoryRepositoryIsReturned()
        {
            // GIVEN
            HistoryRepositoryModel expectedHistoryRepository = new("Repository1");
            MockedHistoryRepositoryService.GetHistoryRepository("Project", $"{expectedHistoryRepository.Name}")
                .Returns(expectedHistoryRepository);
            // WHEN
            var result = await client.GetAsync($"/history-repositories/Project/{expectedHistoryRepository.Name}");

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            HistoryRepositoryModel? historyRepository = await result.Content.ReadFromJsonAsync<HistoryRepositoryModel>();
            historyRepository.ShouldBeEquivalentTo(expectedHistoryRepository);
        }

        [Fact]
        public async Task WhenGettingUnknownHistoryRepository_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedHistoryRepositoryService.GetHistoryRepository("Project", "UnknownRepository")
                .Throws<UnknownEntityException>();
            // WHEN
            var result = await client.GetAsync("/history-repositories/Project/UnknownRepository");

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenProject_WhenCreatingHistoryRepository_ThenHistoryRepositoryIsCreated()
        {
            // GIVEN
            HistoryRepositoryModel expectedHistoryRepository = new("Repository1");
            MockedHistoryRepositoryService.CreateHistoryRepository("Project", expectedHistoryRepository)
                .Returns(expectedHistoryRepository);

            // WHEN
            var result = await client.PostAsJsonAsync($"/history-repositories/Project", expectedHistoryRepository);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            HistoryRepositoryModel? historyRepository = await result.Content.ReadFromJsonAsync<HistoryRepositoryModel>();
            historyRepository.ShouldBeEquivalentTo(expectedHistoryRepository);
        }

        [Fact]
        public async Task WhenCreatingHistoryRepositoryOnUnknownProject_ThenNotFoundIsReturned()
        {
            // GIVEN
            HistoryRepositoryModel expectedHistoryRepository = new("Repository1");
            MockedHistoryRepositoryService.CreateHistoryRepository("UnknownProject", expectedHistoryRepository)
                .Throws<UnknownEntityException>();

            // WHEN
            var result = await client.PostAsJsonAsync($"/history-repositories/UnknownProject", expectedHistoryRepository);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenCreatingHistoryRepositoryOnNonWipProject_ThenBadRequestIsReturned()
        {
            // GIVEN
            HistoryRepositoryModel expectedHistoryRepository = new("Repository1");
            MockedHistoryRepositoryService.CreateHistoryRepository("RunnableProject", expectedHistoryRepository)
                .Throws<NonWipProjectException>();

            // WHEN
            var result = await client.PostAsJsonAsync($"/history-repositories/RunnableProject", expectedHistoryRepository);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenProject_WhenUpdatingHistoryRepository_ThenHistoryRepositoryIsUpdated()
        {
            // GIVEN
            HistoryRepositoryModel expectedHistoryRepository = new("Repository1");
            MockedHistoryRepositoryService.UpdateHistoryRepository("Project", expectedHistoryRepository.Name, expectedHistoryRepository)
                .Returns(expectedHistoryRepository);

            // WHEN
            var result = await client.PutAsJsonAsync($"/history-repositories/Project/{expectedHistoryRepository.Name}", expectedHistoryRepository);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            HistoryRepositoryModel? historyRepository = await result.Content.ReadFromJsonAsync<HistoryRepositoryModel>();
            historyRepository.ShouldBeEquivalentTo(expectedHistoryRepository);
        }

        [Fact]
        public async Task WhenUpdatingHistoryRepositoryOnUnknownProject_ThenNotFoundIsReturned()
        {
            // GIVEN
            HistoryRepositoryModel expectedHistoryRepository = new("Repository1");
            MockedHistoryRepositoryService.UpdateHistoryRepository("UnknownProject", expectedHistoryRepository.Name, expectedHistoryRepository)
                .Throws<UnknownEntityException>();

            // WHEN
            var result = await client.PutAsJsonAsync($"/history-repositories/UnknownProject/{expectedHistoryRepository.Name}", expectedHistoryRepository);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenUpdatingHistoryRepositoryOnNonWipProject_ThenBadRequestIsReturned()
        {
            // GIVEN
            HistoryRepositoryModel expectedHistoryRepository = new("Repository1");
            MockedHistoryRepositoryService.UpdateHistoryRepository("RunnableProject", expectedHistoryRepository.Name, expectedHistoryRepository)
                .Throws<NonWipProjectException>();

            // WHEN
            var result = await client.PutAsJsonAsync($"/history-repositories/RunnableProject/{expectedHistoryRepository.Name}", expectedHistoryRepository);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenProjectWithHistoryRepository_WhenDeletingHistoryRepository_ThenHistoryRepositoryIsDeleted()
        {
            // GIVEN
            HistoryRepositoryModel expectedHistoryRepository = new("Repository1");

            // WHEN
            var result = await client.DeleteAsync($"/history-repositories/Project/{expectedHistoryRepository.Name}");

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task WhenDeletingUnknownHistoryRepository_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedHistoryRepositoryService.When(fake => fake.DeleteHistoryRepository("Project", "UnknownRepository"))
                .Do(call => { throw new UnknownEntityException(); });

            // WHEN
            var result = await client.DeleteAsync("/history-repositories/Project/UnknownRepository");

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenDeletingHistoryRepositoryOnNonWipProject_ThenBadRequestIsReturned()
        {
            // GIVEN
            MockedHistoryRepositoryService.When(fake => fake.DeleteHistoryRepository("RunnableProject", "Repository"))
                .Do(call => { throw new NonWipProjectException(); });

            // WHEN
            var result = await client.DeleteAsync("/history-repositories/RunnableProject/Repository");

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
