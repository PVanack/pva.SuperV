using Shouldly;

namespace pva.SuperV.ApiTests
{
    public class OpenApiTests
    {
        public class ConsoleWriter(ITestOutputHelper output) : StringWriter
        {
            public override void WriteLine(string? value) => output.WriteLine(value);
        }

        private readonly TestProjectApplication application;
        private readonly HttpClient client;

        public OpenApiTests(ITestOutputHelper output)
        {
            application = new();
            client = application.CreateClient();
            Console.SetOut(new ConsoleWriter(output));
        }

        [Fact]
        public async Task GettingOpenApiSwagger_ReturnsSwagger()
        {
            var response = await client.GetAsync("/openapi/v1.json", TestContext.Current.CancellationToken);

            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            string swaggerJson = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            swaggerJson.ShouldNotBeNullOrEmpty();
            await File.WriteAllTextAsync("../../../../pva.SuperV.Api/openapi/swagger.json", swaggerJson, TestContext.Current.CancellationToken);
        }
    }
}
