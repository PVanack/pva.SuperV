using pva.SuperV.Api.Services.Scripts;

using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.FieldProcessings;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class ScriptServiceTests : SuperVTestsBase
    {
        private readonly ScriptService scriptService;
        private readonly RunnableProject runnableProject;
        private readonly WipProject wipProject;

        public ScriptServiceTests()
        {
            scriptService = new(LoggerFactory);
            runnableProject = CreateRunnableProject();
            wipProject = CreateWipProject(null);
        }

        [Fact]
        public async Task GetScripts_ShouldReturnListOfScripts()
        {
            // Act
            var result = await scriptService.GetScriptsAsync(runnableProject.GetId());

            // Assert
            result.Count.ShouldBe(1);
            result.ShouldContain(c => c.Name == ScriptName);
        }

        [Fact]
        public async Task GetScript_ShouldReturnScript_WhenScriptExists()
        {
            // Act
            var result = await scriptService.GetScriptAsync(runnableProject.GetId(), ScriptName);

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe(ScriptName);
        }

        [Fact]
        public async Task GetScript_ShouldThrowUnknownEntityException_WhenScriptDoesNotExist()
        {
            // Act & Assert
            await Assert.ThrowsAsync<UnknownEntityException>(async ()
                => await scriptService.GetScriptAsync(runnableProject.GetId(), "UnknownScript"));
        }

        [Fact]
        public async Task CreateScriptInWipProject_ShouldCreateScript()
        {
            ScriptDefinitionModel expectedScript = new("NewScript", TestTopicName, "");
            // Act & Assert
            ScriptDefinitionModel createScriptDefinitionModel = await scriptService.CreateScriptAsync(wipProject.GetId(), expectedScript);

            createScriptDefinitionModel.ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedScript);
        }

        [Fact]
        public async Task UpdateScriptInWipProject_ShouldUpdateScript()
        {
            ScriptDefinitionModel expectedScript = new(ScriptName, TestTopicName, "// Test");
            // Act & Assert
            ScriptDefinitionModel createScriptDefinitionModel = await scriptService.UpdateScriptAsync(wipProject.GetId(), expectedScript.Name, expectedScript);

            createScriptDefinitionModel.ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedScript);
        }

        [Fact]
        public async Task DeleteScriptInWipProject_ShouldDeleteScript()
        {
            ScriptDefinitionModel expectedScript = new("NewScript", TestTopicName, "// Test");
            _ = await scriptService.CreateScriptAsync(wipProject.GetId(), expectedScript);
            // Act
            await scriptService.DeleteScriptAsync(wipProject.GetId(), expectedScript.Name);

            // Assert
            wipProject.ScriptDefinitions.ShouldNotContainKey(expectedScript.Name);
        }
    }
}
