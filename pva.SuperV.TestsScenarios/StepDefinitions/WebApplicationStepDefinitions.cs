namespace pva.SuperV.TestsScenarios.StepDefinitions
{
    [Binding]
    public class WebApplicationStepDefinitions
    {
        private readonly ScenarioContext scenarioContext;

        public WebApplicationStepDefinitions(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [Given("REST application is started")]
        public void RestApplicationIsStarted()
        {
            TestProjectApplication testProjectApplication = new();
            scenarioContext.Add(ScenarioContextExtensions.RestApplication, testProjectApplication);
            HttpClient client = testProjectApplication.CreateClient();
            scenarioContext.Add(ScenarioContextExtensions.WebClient, client);
        }
    }
}
