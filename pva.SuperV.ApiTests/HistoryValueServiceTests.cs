using pva.SuperV.Api.Services.History;
using pva.SuperV.Engine;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.HistoryRetrieval;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class HistoryValueServiceTests : SuperVTestsBase
    {
        private readonly HistoryValuesService historyValuesService;
        private readonly RunnableProject runnableProject;

        public HistoryValueServiceTests()
        {
            historyValuesService = new();
            WipProject wipProject = CreateWipProject(TDengineHistoryStorage.Prefix);
            runnableProject = Task.Run(async () => await Project.BuildAsync(wipProject)).Result;
            runnableProject.CreateInstance(ClassName, InstanceName);
            Project.Unload(wipProject);
        }

        [Fact]
        public void GivenInstamceWithHistory_WhenGettingHistory_ThenHistoryRowsAreReturned()
        {
            // Given
            DateTime timestamp = DateTime.UtcNow;
            HistoryRawResultModel expectedHistoryResult = new([new HistoryFieldModel(ValueFieldName, "System.Int32", 0)],
                [new HistoryRawRowModel(timestamp, null, null, null, QualityLevel.Good, [123456])]);

            runnableProject.SetInstanceValue<int>(InstanceName, ValueFieldName, 123456, timestamp);
            // Act
            HistoryRequestModel request = new(timestamp.AddSeconds(-1), DateTime.Now, null, null, [ValueFieldName]);
            HistoryRawResultModel historyResult = historyValuesService!.GetInstanceRawHistoryValues(runnableProject.GetId(), InstanceName, request);

            // Assert
            // This doesn' work, as comparison of the object values use Object Equals().
            //historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
            // As a workaround, we compare each element :-(
            historyResult!.Header.ShouldBeEquivalentTo(expectedHistoryResult.Header);
            historyResult!.Rows.Count.ShouldBe(expectedHistoryResult.Rows.Count);
            for (int rowIndex = 0; rowIndex < expectedHistoryResult.Rows.Count; rowIndex++)
            {
                var actualRow = historyResult!.Rows[rowIndex];
                var expectedRow = expectedHistoryResult.Rows[rowIndex];
                actualRow.Timestamp.ShouldBe(expectedRow.Timestamp);
                actualRow.Quality.ShouldBe(expectedRow.Quality);
                actualRow.StartTime.ShouldBe(expectedRow.StartTime);
                actualRow.EndTime.ShouldBe(expectedRow.EndTime);
                actualRow.Duration.ShouldBe(expectedRow.Duration);
                actualRow.FieldValues.Count.ShouldBe(expectedRow.FieldValues.Count);
                for (int fieldIndex = 0; fieldIndex < actualRow.FieldValues.Count; fieldIndex++)
                {
                    int actualValue = (int)actualRow.FieldValues[fieldIndex];
                    int expectedValue = (int)expectedRow.FieldValues[fieldIndex];
                    actualValue.ShouldBe(expectedValue);
                }
            }

        }
    }
}
