using pva.SuperV.Engine;
using pva.SuperV.Engine.HistoryStorage;
using Shouldly;

namespace pva.SuperV.EngineTests
{
    [Collection("Project building")]
    public class TDengineTests
    {
        [Fact]
        public void GivenProjectWithClassInstance_WhenSettingFieldValue_ThenValueIsHistorized()
        {
            // GIVEN
            RunnableProject runnableProject = ProjectHelpers.CreateRunnableProject(HistoryStorageEngineFactory.TdEngineHistoryStorage);
            dynamic? instance = runnableProject.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);
            DateTime testStart = DateTime.UtcNow;
            // WHEN
            instance!.Value.SetValue(50);
            DateTime ts1 = instance!.Value.Timestamp;
            instance!.Value.SetValue(110);
            DateTime ts2 = instance!.Value.Timestamp;

            // THEN
            List<string> fields = [ProjectHelpers.ValueFieldName];
            List<HistoryRow> rows = runnableProject.GetHistoryValues(instance.Name, testStart, DateTime.Now, fields);
            rows.Count.ShouldBe(2);
            rows[0].Ts.ShouldBe(ts1);
            rows[0].Values.Count.ShouldBe(1);
            rows[0].GetValue<int>(0).ShouldBe(50);

            rows[1].Values.Count.ShouldBe(1);
            rows[1].Ts.ShouldBe(ts2);
            rows[1].GetValue<int>(0).ShouldBe(110);

            instance?.Dispose();
            ProjectHelpers.DeleteProject(runnableProject);
        }
    }
}
