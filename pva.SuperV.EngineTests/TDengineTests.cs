﻿using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.HistoryRetrieval;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Engine.Processing;
using Shouldly;

namespace pva.SuperV.EngineTests
{
    [Collection("Project building")]
    public class TDengineTests : SuperVTestsBase
    {
        private readonly WipProject wipProject;
        private RunnableProject? runnableProject;

        public TDengineTests()
        {
            wipProject = CreateWipProject(TDengineHistoryStorage.Prefix);
        }

        [Fact]
        public async Task GivenProjectWithClassInstance_WhenSettingFieldValue_ThenValueIsHistorized()
        {
            // GIVEN
            runnableProject = await Project.BuildAsync(wipProject);
            dynamic? instance = runnableProject.CreateInstance(ClassName, InstanceName);
            DateTime testStart = DateTime.UtcNow;
            // WHEN
            instance!.Value.SetValue(50);
            DateTime ts1 = instance!.Value.Timestamp;
#pragma warning disable S2925 // "Thread.Sleep" should not be used in tests
            Thread.Sleep(50);
#pragma warning restore S2925 // "Thread.Sleep" should not be used in tests
            instance!.Value.SetValue(110);
            DateTime ts2 = instance!.Value.Timestamp;

            // THEN
            List<string> fields = [ValueFieldName];
            HistoryTimeRange query = new(testStart, DateTime.UtcNow);
            List<HistoryRow> rows = runnableProject.GetHistoryValues(instance.Name, query, fields);
            rows.Count.ShouldBe(2);
            rows[0].Ts.ShouldBe(ts1);
            rows[0].Quality.ShouldBe(QualityLevel.Good);
            rows[0].Values.Count.ShouldBe(1);
            rows[0].GetValue<int>(0).ShouldBe(50);

            rows[1].Values.Count.ShouldBe(1);
            rows[1].Ts.ShouldBe(ts2);
            rows[1].Quality.ShouldBe(QualityLevel.Good);
            rows[1].GetValue<int>(0).ShouldBe(110);

            instance?.Dispose();
        }


        [Fact]
        public async Task GivenUnhandledFieldUsedInHistorizationProcessing_WhenBuildingProject_ThenExceptionIsThrown()
        {
            // GIVEN
            Class clazz = wipProject.GetClass(ClassName);
            clazz.AddField(new FieldDefinition<List<int>>("ListInt"));
            wipProject.AddFieldChangePostProcessing(ClassName, ValueFieldName,
                new HistorizationProcessing<int>("BadHistProcessing", wipProject, clazz, ValueFieldName, HistoryRepositoryName, null, ["ListInt"]));
            await Assert.ThrowsAsync<UnhandledHistoryFieldTypeException>(async () => await Project.BuildAsync(wipProject));
        }

        [Fact]
        public async Task GivenInstanceWithHistory_WhenGettingHistoryValues_ThenValuesAreReturned()
        {
            // GIVEN
            runnableProject = await Project.BuildAsync(wipProject);
            dynamic? instance = runnableProject.CreateInstance(ClassName, InstanceName);
            // WHEN
            DateTime ts1 = DateTime.Now;
            instance!.Value.SetValue(50, ts1, QualityLevel.Good);
            DateTime ts2 = DateTime.Now;
            instance!.Value.SetValue(100, ts2, QualityLevel.Good);

            // THEN
            HistoryTimeRange query = new(ts1, ts2.AddMinutes(59));
            List<HistoryRow> rows = runnableProject.GetHistoryValues(instance.Name, query, new List<string>([ValueFieldName]));
            rows.Count.ShouldBe(2);
            rows[0].Ts.ShouldBe(ts1.ToUniversalTime());
            rows[0].Values.Count.ShouldBe(1);
            rows[0].GetValue<int>(0).ShouldBe(50);

            rows[1].Ts.ShouldBe(ts2.ToUniversalTime());
            rows[1].Values.Count.ShouldBe(1);
            rows[1].GetValue<int>(0).ShouldBe(100);

            instance?.Dispose();
        }

        [Fact]
        public async Task GivenStartTimeEqualToEndTime_WhenGettingHistoryValues_ThenExceptionIsThrown()
        {
            // GIVEN
            runnableProject = await Project.BuildAsync(wipProject);
            dynamic? instance = runnableProject.CreateInstance(ClassName, InstanceName);
            // WHEN
            DateTime endTime = DateTime.Now;

            // THEN
            HistoryTimeRange query = new(endTime, endTime);
            await Assert.ThrowsAsync<BadHistoryStartTimeException>(async ()
                => _ = await runnableProject?.GetHistoryValues(instance!.Name, query, new List<string>([ValueFieldName])));
            instance?.Dispose();

        }

        [Fact]
        public async Task GivenInstanceWithHistory_WhenGettingStatisticValues_ThenStatisticsAreReturned()
        {
            // GIVEN
            runnableProject = await Project.BuildAsync(wipProject);
            dynamic? instance = runnableProject.CreateInstance(ClassName, InstanceName);
            // WHEN
            DateTime ts1 = ParseDateTime("2025-03-01T00:00:00Z");
            instance!.Value.SetValue(50, ts1, QualityLevel.Good);
            instance!.Value.SetValue(150, ts1.AddMinutes(15), QualityLevel.Good);
            DateTime ts2 = ParseDateTime("2025-03-01T01:00:00Z");
            instance!.Value.SetValue(100, ts2, QualityLevel.Good);

            // THEN
            List<HistoryStatisticFieldName> fields =
                [
                new(ValueFieldName, HistoryStatFunction.AVG),
                new(ValueFieldName, HistoryStatFunction.TWA)
                ];
            HistoryStatisticTimeRange query = new(ts1, ts2.AddMinutes(59), new TimeSpan(1, 0, 0), FillMode.LINEAR);
            List<HistoryStatisticRow> rows = runnableProject.GetHistoryStatistics(instance.Name, query, fields);
            rows.Count.ShouldBe(2);
            rows[0].Ts.ShouldBe(ts1.ToUniversalTime());
            rows[0].StartTime.ShouldBe(ts1.ToUniversalTime());
            rows[0].EndTime.ShouldBe(ts1.Add(query.Interval).ToUniversalTime());
            rows[0].Duration.Ticks.ShouldBe(query.Interval.Ticks);
            rows[0].Values.Count.ShouldBe(2);
            rows[0].GetValue<double>(0).ShouldBe(100);
            rows[0].GetValue<double>(1).ShouldBe(118.74999999999827); // Should be 125

            rows[1].Ts.ShouldBe(ts2.ToUniversalTime());
            rows[1].StartTime.ShouldBe(ts2.ToUniversalTime());
            rows[1].EndTime.ShouldBe(ts2.Add(query.Interval).ToUniversalTime());
            rows[1].Duration.Ticks.ShouldBe(query.Interval.Ticks);
            rows[1].Values.Count.ShouldBe(2);
            rows[1].GetValue<double>(0).ShouldBe(100);
            rows[1].GetValue<double>(1).ShouldBe(100);

            instance?.Dispose();
        }

        [Fact]
        public async Task WhenGettingStatisticValuesWithStartTimeEqualToEndTime_ThenExceptionIsThrown()
        {
            // GIVEN
            runnableProject = await Project.BuildAsync(wipProject);
            dynamic? instance = runnableProject.CreateInstance(ClassName, InstanceName);
            // WHEN
            DateTime endTime = DateTime.Now;

            // THEN
            List<HistoryStatisticFieldName> fields =
                [
                new(ValueFieldName, HistoryStatFunction.AVG)
                ];
            HistoryStatisticTimeRange query = new(endTime, endTime, new TimeSpan(1, 0, 0), FillMode.LINEAR);
            await Assert.ThrowsAsync<BadHistoryStartTimeException>(async ()
                => _ = await runnableProject?.GetHistoryStatistics(instance!.Name, query, fields));

            instance?.Dispose();
        }

        [Fact]
        public async Task WhenGettingStatisticValuesWithIntervalGreaterToEndTimeMinusStartTime_ThenExceptionIsThrown()
        {
            // GIVEN
            runnableProject = await Project.BuildAsync(wipProject);
            dynamic? instance = runnableProject.CreateInstance(ClassName, InstanceName);
            // WHEN
            DateTime endTime = DateTime.Now;
            DateTime startTime = endTime.AddHours(-1);

            // THEN
            List<HistoryStatisticFieldName> fields =
                [
                new(ValueFieldName, HistoryStatFunction.AVG)
                ];
            HistoryStatisticTimeRange query = new(startTime, endTime, TimeSpan.FromDays(1), FillMode.LINEAR);
            await Assert.ThrowsAsync<BadHistoryIntervalException>(async ()
                => _ = await runnableProject?.GetHistoryStatistics(instance!.Name, query, fields));

            instance?.Dispose();
        }
    }
}
