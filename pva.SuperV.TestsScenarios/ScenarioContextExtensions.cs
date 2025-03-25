namespace pva.SuperV.TestsScenarios
{
    public static class ScenarioContextExtensions
    {
        public const string RestApplication = nameof(RestApplication);
        public const string WebClient = nameof(WebClient);

        public static HttpClient GetWebClient(this ScenarioContext scenarioContext)
        {
            return scenarioContext.Get<HttpClient>(WebClient);
        }
    }
}
