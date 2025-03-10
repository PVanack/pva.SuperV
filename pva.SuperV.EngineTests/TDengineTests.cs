﻿using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Engine.Processing;
using Shouldly;

namespace pva.SuperV.EngineTests
{
    [Collection("Project building")]
    public class TDengineTests
    {
        private readonly WipProject wipProject;
        private RunnableProject? runnableProject;

        public TDengineTests()
        {
            wipProject = ProjectHelpers.CreateWipProject(TDengineHistoryStorage.Prefix);
        }

        [Fact]
        public async Task GivenProjectWithClassInstance_WhenSettingFieldValue_ThenValueIsHistorized()
        {
            // GIVEN
            runnableProject = await Project.BuildAsync(wipProject);
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
        }

        [Fact]
        public async Task GivenTimespanFieldUsedInHistorizationProcessing_WhenBuildingProject_ThenExceptionIsThrown()
        {
            // GIVEN
            Class clazz = wipProject.GetClass(ProjectHelpers.ClassName);
            clazz.AddField(new FieldDefinition<TimeSpan>("TimeSpanField"));
            wipProject.AddFieldChangePostProcessing(ProjectHelpers.ClassName, ProjectHelpers.ValueFieldName,
                new HistorizationProcessing<int>("BadHistProcessing", wipProject, clazz, ProjectHelpers.ValueFieldName, ProjectHelpers.HistoryRepositoryName, null, ["TimeSpanField"]));
            await Assert.ThrowsAsync<UnhandledHistoryFieldTypeException>(async () => await Project.BuildAsync(wipProject));
        }
    }
}
