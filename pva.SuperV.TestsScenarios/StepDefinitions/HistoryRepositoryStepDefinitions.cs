using pva.SuperV.Model.HistoryRepositories;
using Shouldly;
using System.Net.Http.Json;

namespace pva.SuperV.TestsScenarios.StepDefinitions
{
    [Binding]
    public class HistoryRepositoryStepDefinitions(ScenarioContext scenarioContext) : BaseStepDefinition(scenarioContext)
    {
        [Given("History repository {string} is created in project {string}")]
        public async ValueTask HistoryRepositoryIsCreated(string historyRepositoryName, string projectId)
        {
            HistoryRepositoryModel expectedHistoryRepository = new(historyRepositoryName);
            var result = await Client.PostAsJsonAsync($"/history-repositories/{projectId}", expectedHistoryRepository);

            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            HistoryRepositoryModel? historyRepository = await result.Content.ReadFromJsonAsync<HistoryRepositoryModel>();
            historyRepository.ShouldBeEquivalentTo(expectedHistoryRepository);
        }
    }
}
