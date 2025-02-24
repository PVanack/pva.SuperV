using NSubstitute;
using pva.SuperV.Api;
using pva.SuperV.Model.HistoryRepositories;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
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
        private IHistoryRepositoryService MockHistoryRepositoryService { get => application.MockHistoryRepositoryService!; }

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
            MockHistoryRepositoryService.GetHistoryRepositories("Project")
                .Returns(expectedHistoryRepositories);
            // WHEN
            var result = await client.GetAsync("/history-repositories/Project");

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            HistoryRepositoryModel[]? historyRepositories = await result.Content.ReadFromJsonAsync<HistoryRepositoryModel[]>();
            historyRepositories.ShouldBeEquivalentTo(expectedHistoryRepositories.ToArray());
        }

        [Fact]
        public async Task GivenProjectWithHistoryRepositories_WhenGettingHistoryRepository_ThenHistoryRepositoryIsReturned()
        {
            // GIVEN
            HistoryRepositoryModel expectedHistoryRepository = new("Repository1");
            MockHistoryRepositoryService.GetHistoryRepository("Project", $"{expectedHistoryRepository.Name}")
                .Returns(expectedHistoryRepository);
            // WHEN
            var result = await client.GetAsync($"/history-repositories/Project/{expectedHistoryRepository.Name}");

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            HistoryRepositoryModel? historyRepository = await result.Content.ReadFromJsonAsync<HistoryRepositoryModel>();
            historyRepository.ShouldBeEquivalentTo(expectedHistoryRepository);
        }

        [Fact]
        public async Task GivenProject_WhenCreatingHistoryRepository_ThenHistoryRepositoryIsCreated()
        {
            // GIVEN
            HistoryRepositoryModel expectedHistoryRepository = new("Repository1");
            MockHistoryRepositoryService.CreateHistoryRepository("Project", expectedHistoryRepository)
                .Returns(expectedHistoryRepository);

            // WHEN
            var result = await client.PostAsJsonAsync($"/history-repositories/Project", expectedHistoryRepository);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            HistoryRepositoryModel? historyRepository = await result.Content.ReadFromJsonAsync<HistoryRepositoryModel>();
            historyRepository.ShouldBeEquivalentTo(expectedHistoryRepository);
        }
    }
}
