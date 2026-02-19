using pva.SuperV.Engine;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Engine.Processing;

namespace pva.SuperV.EngineTests
{
    public class ScriptTests : SuperVTestsBase
    {
        private const string TestClassName = "ScriptTestClass";
        private const string TopicName = "ZScriptTestTopic";

        [Fact]
        public async ValueTask GivenFieldAndScript_WhenChangingFieldValue_ThenScriptIsExecuted()
        {
            // GIVEN
            WipProject wipProject = CreateWipProject(NullHistoryStorageEngine.Prefix);
            _ = wipProject.AddClass(TestClassName);
            wipProject.AddField(TestClassName, new FieldDefinition<int>(IntFieldWithTopicName, 10, TopicName));
            wipProject.AddField(TestClassName, new FieldDefinition<int>(ValueFieldName, 10));
            const string scriptSource = @"
// The script can reference fields from the instance itself without specifying the instance or fields from other instances by naming the instance and the field with a .
// Variables are also accessible:
// - RunnableProject project: the actual project,
// - Dictionary<string, IInstance> instances: the instances dictionary used in the script,
// - FieldValueChangedEvent fieldValueChangedEvent: the field value change event information:
//   - string TopicName: the name of the topic
//   - IField Field: the field which changed value
//   - dynamic PreviousValue: field previous value
//   - dynamic NewValue: field new value

{{Value}} = {{ScriptTestInstance.IntFieldWithTopic}};
";
            ScriptDefinition script = new("ZScript", TopicName, scriptSource);
            wipProject.AddScript(script);
            RunnableProject project = await Project.BuildAsync(wipProject);
            var instance = project.CreateInstance(TestClassName, "ScriptTestInstance") as dynamic;

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
    }
}
