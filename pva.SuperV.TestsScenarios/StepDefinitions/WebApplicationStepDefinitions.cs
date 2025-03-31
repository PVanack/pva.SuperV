namespace pva.SuperV.TestsScenarios.StepDefinitions
{
    [Binding]
    public class WebApplicationStepDefinitions(ScenarioContext ScenarioContext)
    {
        [Given("REST application is started")]
        public void RestApplicationIsStarted()
        {
            TestProjectApplication testProjectApplication = new();
            ScenarioContext.Add(ScenarioContextExtensions.RestApplication, testProjectApplication);
            HttpClient client = testProjectApplication.CreateClient();
            ScenarioContext.Add(ScenarioContextExtensions.WebClient, client);
        }
    }
}
