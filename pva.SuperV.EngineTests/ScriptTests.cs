using pva.SuperV.Engine;

namespace pva.SuperV.EngineTests
{
    public class ScriptTests : SuperVTestsBase
    {
        [Fact]
        public async ValueTask GivenFieldAndScript_WhenChangingFieldValue_ThenScriptIsExecuted()
        {
            // GIVEN
            RunnableProject project = CreateRunnableProject();
            var instance = project.CreateInstance(ClassWithTopicName, InstanceWithTopicName) as dynamic;

            // WHEN
            instance!.IntFieldWithTopic.Value = 42;

            // THEN
            await WaitForCondition(1000, () => instance.Value.Value == 42);
            Assert.Equal(42, instance.IntFieldWithTopic.Value);
            Assert.Equal(42, instance.Value.Value);

            // WHEN
            instance!.IntFieldWithTopic.Value = 63;

            // THEN
            await WaitForCondition(1000, () => instance.Value.Value == 63);
            Assert.Equal(63, instance.IntFieldWithTopic.Value);
            Assert.Equal(63, instance.Value.Value);

            instance.Dispose();
            DeleteProject(project);
        }

        private static async ValueTask WaitForCondition(int timeoout, Func<bool> condition)
        {
            const int waitInterval = 100;
            while (timeoout > 0)
            {
                if (condition())
                {
                    return;
                }
                await Task.Delay(waitInterval);
                timeoout -= waitInterval;
            }
        }
    }
}
