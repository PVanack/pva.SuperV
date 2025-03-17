using pva.SuperV.Api.Services.History;
using pva.SuperV.Engine;
using pva.SuperV.Engine.HistoryRetrieval;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.HistoryRetrieval;
using pva.SuperV.Model.Instances;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    public class HistoryValueServiceTests : SuperVTestsBase
    {
        // Set it to null to run the tests but only on their own.
        private const string SkipReason = "Not working when run as part of the whole test project";
        private readonly HistoryValuesService historyValuesService;
        private readonly WipProject wipProject;
        private readonly RunnableProject runnableProject;

        public HistoryValueServiceTests()
        {
            historyValuesService = new();
            wipProject = CreateWipProject(TDengineHistoryStorage.Prefix);
            runnableProject = Task.Run(async () => await Project.BuildAsync(wipProject)).Result;
            runnableProject.CreateInstance(ClassName, InstanceName);
        }

        [Fact(Skip = SkipReason)]
        public void GivenInstanceWithHistory_WhenGettingHistoryRawValues_ThenHistoryRawRowsAreReturned()
        {
            // Given
            DateTime timestamp = DateTime.UtcNow;
            HistoryRawResultModel expectedHistoryResult = new([new HistoryFieldModel(ValueFieldName, "System.Int32", 0)],
                [new HistoryRawRowModel(timestamp, QualityLevel.Good, [123456])]);
            runnableProject.SetInstanceValue<int>(InstanceName, ValueFieldName, 123456, timestamp);

            // Act
            HistoryRequestModel request = new(timestamp.AddSeconds(-1), DateTime.Now, [ValueFieldName]);
            HistoryRawResultModel historyResult = historyValuesService.GetInstanceRawHistoryValues(runnableProject.GetId(), InstanceName, request);

            // Assert
            // This doesn' work, as comparison of the object values uses Object Equals().
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
                actualRow.FieldValues.Count.ShouldBe(expectedRow.FieldValues.Count);
                for (int fieldIndex = 0; fieldIndex < actualRow.FieldValues.Count; fieldIndex++)
                {
                    int actualValue = (int)actualRow.FieldValues[fieldIndex];
                    int expectedValue = (int)expectedRow.FieldValues[fieldIndex];
                    actualValue.ShouldBe(expectedValue);
                }
            }
        }

        [Fact(Skip = SkipReason)]
        public void GivenInstanceWithHistory_WhenGettingHistoryValues_ThenHistoryRowsAreReturned()
        {
            // Given
            DateTime timestamp = DateTime.UtcNow;
            HistoryResultModel expectedHistoryResult = new([new HistoryFieldModel(ValueFieldName, "System.Int32", 0)],
                [new HistoryRowModel(timestamp, QualityLevel.Good, [new IntFieldValueModel(123456, null, QualityLevel.Good, timestamp)])]);

            runnableProject.SetInstanceValue<int>(InstanceName, ValueFieldName, 123456, timestamp);
            // Act
            HistoryRequestModel request = new(timestamp.AddSeconds(-1), DateTime.Now, [ValueFieldName]);
            HistoryResultModel historyResult = historyValuesService.GetInstanceHistoryValues(runnableProject.GetId(), InstanceName, request);

            // Assert
            // This doesn' work, as comparison of the object values use Object Equals().
            historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
        }

        [Fact(Skip = SkipReason)]
        public void GivenInstanceWithHistory_WhenGettingHistoryRawStatistics_ThenHistoryRawRowsAreReturned()
        {
            // Given
            DateTime timestamp = DateTime.UtcNow.Date;
            HistoryStatisticsRawResultModel expectedHistoryResult = new([new HistoryStatisticResultFieldModel(ValueFieldName, "System.Int32", 0, HistoryStatFunction.AVG)],
                [new HistoryStatisticsRawRowModel(timestamp, timestamp, timestamp.AddHours(1), TimeSpan.FromHours(1), QualityLevel.Good, [123456])]);
            runnableProject.SetInstanceValue<int>(InstanceName, ValueFieldName, 123456, timestamp);

            // Act
            HistoryStatisticsRequestModel request = new(timestamp, timestamp.AddMinutes(59), TimeSpan.FromHours(1), FillMode.PREV,
                [new HistoryStatisticFieldModel(ValueFieldName, HistoryStatFunction.AVG)]);
            HistoryStatisticsRawResultModel historyResult = historyValuesService.GetInstanceRawHistoryStatistics(runnableProject.GetId(), InstanceName, request);

            // Assert
            // This doesn' work, as comparison of the object values uses Object Equals().
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
                actualRow.FieldValues.Count.ShouldBe(expectedRow.FieldValues.Count);
                for (int fieldIndex = 0; fieldIndex < actualRow.FieldValues.Count; fieldIndex++)
                {
                    int actualValue = (int)actualRow.FieldValues[fieldIndex];
                    int expectedValue = (int)expectedRow.FieldValues[fieldIndex];
                    actualValue.ShouldBe(expectedValue);
                }
            }
        }

        [Fact(Skip = SkipReason)]
        public void GivenInstanceWithHistory_WhenGettingHistoryStatistics_ThenHistoryRowsAreReturned()
        {
            // Given
            DateTime timestamp = DateTime.UtcNow.Date;
            HistoryStatisticsResultModel expectedHistoryResult = new([new HistoryStatisticResultFieldModel(ValueFieldName, "System.Int32", 0, HistoryStatFunction.AVG)],
                [new HistoryStatisticsRowModel(timestamp, timestamp, timestamp.AddHours(1), TimeSpan.FromHours(1), QualityLevel.Good,
                    [new IntFieldValueModel(123456, null, QualityLevel.Good, timestamp)])]);

            runnableProject.SetInstanceValue<int>(InstanceName, ValueFieldName, 123456, timestamp);
            // Act
            HistoryStatisticsRequestModel request = new(timestamp, timestamp.AddMinutes(59), TimeSpan.FromHours(1), FillMode.PREV,
                [new HistoryStatisticFieldModel(ValueFieldName, HistoryStatFunction.AVG)]);
            HistoryStatisticsResultModel historyResult = historyValuesService.GetInstanceHistoryStatistics(runnableProject.GetId(), InstanceName, request);

            // Assert
            // This doesn' work, as comparison of the object values use Object Equals().
            historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
        }
    }
}
