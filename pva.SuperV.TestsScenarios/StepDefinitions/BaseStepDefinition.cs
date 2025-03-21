namespace pva.SuperV.TestsScenarios.StepDefinitions
{
    public abstract class BaseStepDefinition
    {
        protected ScenarioContext ScenarioContext { get; init; }
        protected HttpClient Client { get; init; }

        protected BaseStepDefinition(ScenarioContext scenarioContext)
        {
            ScenarioContext = scenarioContext;
            Client = ScenarioContext.GetWebClient();
        }
    }
}
