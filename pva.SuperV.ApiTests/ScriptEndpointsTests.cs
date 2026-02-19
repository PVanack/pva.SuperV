using NSubstitute;
using NSubstitute.ExceptionExtensions;
using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;
using Shouldly;
using System.Net.Http.Json;

namespace pva.SuperV.ApiTests
{
    public class ScriptEndpointsTests
    {
        public class ConsoleWriter(ITestOutputHelper output) : StringWriter
        {
            public override void WriteLine(string? value) => output.WriteLine(value!);
        }

        private readonly TestProjectApplication application;
        private readonly HttpClient client;
        private IScriptService MockedScriptService { get => application.MockedScriptService!; }

        public ScriptEndpointsTests(ITestOutputHelper output)
        {
            application = new();
            client = application.CreateClient();
            Console.SetOut(new ConsoleWriter(output));
        }

        [Fact]
        public async Task GivenExistingScriptsInProject_WhenGettingProjectScripts_ThenScriptsAreReturned()
        {
            // GIVEN
            List<ScriptDefinitionModel> expectedScripts = [new ScriptDefinitionModel("Script", "Topic", "")];
            MockedScriptService.GetScriptsAsync("Project1")
                .Returns(expectedScripts);

            // WHEN
            var response = await client.GetAsync("/scripts/Project1", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            ScriptDefinitionModel[]? projectScripts = await response.Content.ReadFromJsonAsync<ScriptDefinitionModel[]>(TestContext.Current.CancellationToken);
            projectScripts.ShouldBeEquivalentTo(expectedScripts.ToArray());
        }

        [Fact]
        public async Task WhenGettingUnknownProjectScript_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedScriptService.GetScriptsAsync("UnknownProject")
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.GetAsync("/scripts/UnknownProject", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenExistingScriptsInProject_WhenGettingProjectScript_ThenScriptIsReturned()
        {
            // GIVEN
            ScriptDefinitionModel expectedScript = new("Script", "Topic", "");
            MockedScriptService.GetScriptAsync("Project1", expectedScript.Name)
                .Returns(expectedScript);

            // WHEN
            var response = await client.GetAsync($"/scripts/Project1/{expectedScript.Name}", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            ScriptDefinitionModel? retrievedScript = await response.Content.ReadFromJsonAsync<ScriptDefinitionModel>(TestContext.Current.CancellationToken);
            retrievedScript.ShouldBeEquivalentTo(expectedScript);
        }

        [Fact]
        public async Task WhenGettingProjectUnknownScript_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedScriptService.GetScriptAsync("Project1", "UnknownScript")
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.GetAsync("/scripts/Project1/UnknownScript", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenWipProject_WhenCreatingProjectScrpit_ThenScriptIsCreated()
        {
            // GIVEN
            ScriptDefinitionModel expectedScript = new("Script", "Topic", "");
            MockedScriptService.CreateScriptAsync("Project1", Arg.Any<ScriptDefinitionModel>())
                .Returns(expectedScript);

            // WHEN
            var response = await client.PostAsJsonAsync("/scripts/Project1", expectedScript, TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            ScriptDefinitionModel? createdScript = await response.Content.ReadFromJsonAsync<ScriptDefinitionModel>(TestContext.Current.CancellationToken);
            createdScript.ShouldBeEquivalentTo(expectedScript);
        }

        [Fact]
        public async Task GivenUnknownProjectProject_WhenCreatingProjectScript_ThenNotFoundIsReturned()
        {
            // GIVEN
            ScriptDefinitionModel expectedScript = new("Script", "Topic", "");
            MockedScriptService.CreateScriptAsync("UnknownProject", Arg.Any<ScriptDefinitionModel>())
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.PostAsJsonAsync("/scripts/UnknownProject", expectedScript, TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenNonWipProject_WhenCreatingProjectScript_ThenBadRequestIsReturned()
        {
            // GIVEN
            ScriptDefinitionModel expectedScript = new("Script", "Topic", "");
            MockedScriptService.CreateScriptAsync("RunnableProject", Arg.Any<ScriptDefinitionModel>())
                .ThrowsAsync<NonWipProjectException>();

            // WHEN
            var response = await client.PostAsJsonAsync("/scripts/RunnableProject", expectedScript, TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task GivenExistingScriptInWipProject_WhenUpdatingProjectScript_ThenScriptIsUpdated()
        {
            // GIVEN
            ScriptDefinitionModel expectedScript = new("Script", "Topic", "");
            MockedScriptService.UpdateScriptAsync("Project1", expectedScript.Name, Arg.Any<ScriptDefinitionModel>())
                .Returns(expectedScript);

            // WHEN
            var response = await client.PutAsJsonAsync($"/scripts/Project1/{expectedScript.Name}", expectedScript, TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            ScriptDefinitionModel? createdScript = await response.Content.ReadFromJsonAsync<ScriptDefinitionModel>(TestContext.Current.CancellationToken);
            createdScript.ShouldBeEquivalentTo(expectedScript);
        }

        [Fact]
        public async Task GivenUnknownProjectProject_WhenUpdatingProjectScript_ThenNotFoundIsReturned()
        {
            // GIVEN
            ScriptDefinitionModel expectedScript = new("Script", "Topic", "");
            MockedScriptService.UpdateScriptAsync("UnknownProject", expectedScript.Name, Arg.Any<ScriptDefinitionModel>())
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.PutAsJsonAsync($"/scripts/UnknownProject/{expectedScript.Name}", expectedScript, TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenNonWipProject_WhenUpdatingProjectScript_ThenBadRequestIsReturned()
        {
            // GIVEN
            ScriptDefinitionModel expectedScript = new("Script", "Topic", "");
            MockedScriptService.UpdateScriptAsync("RunnableProject", expectedScript.Name, Arg.Any<ScriptDefinitionModel>())
                .ThrowsAsync<NonWipProjectException>();

            // WHEN
            var response = await client.PutAsJsonAsync($"/scripts/RunnableProject/{expectedScript.Name}", expectedScript, TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenWipProject_WhenDeletingProjectScript_ThenScriptIsDeleted()
        {
            // GIVEN
            ScriptDefinitionModel expectedScript = new("Script", "Topic", "");

            // WHEN
            var response = await client.DeleteAsync($"/scripts/Project1/{expectedScript.Name}", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task WhenDeletingUnknownProjectScript_ThenNotFoundIsReturned()
        {
            // GIVEN
            ScriptDefinitionModel expectedScript = new("Script", "Topic", "");
            MockedScriptService.When(async (fake) => await fake.DeleteScriptAsync("UnknownProject", expectedScript.Name))
                .Do(_ => throw new UnknownEntityException());

            // WHEN
            var response = await client.DeleteAsync($"/scripts/UnknownProject/{expectedScript.Name}", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenDeletingNonWipProjectScript_ThenBadRequestIsReturned()
        {
            // GIVEN
            ScriptDefinitionModel expectedScript = new("Script", "Topic", "");
            MockedScriptService.When(async (fake) => await fake.DeleteScriptAsync("RunnableProject", expectedScript.Name))
                .Do(_ => throw new NonWipProjectException());

            // WHEN
            var response = await client.DeleteAsync($"/scripts/RunnableProject/{expectedScript.Name}", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

    }
}
