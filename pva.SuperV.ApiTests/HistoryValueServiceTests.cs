using pva.SuperV.Api.Services.History;
using pva.SuperV.Engine;
using pva.SuperV.Engine.HistoryRetrieval;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.HistoryRetrieval;
using pva.SuperV.Model.Instances;
using Shouldly;
using Xunit.Abstractions;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class HistoryValueServiceTests : SuperVTestsBase
    {
        public class ConsoleWriter(ITestOutputHelper output) : StringWriter
        {
            public override void WriteLine(string? value) => output.WriteLine(value);
        }

        private readonly HistoryValuesService historyValuesService;
        private WipProject? wipProject;
        private RunnableProject? runnableProject;


        public HistoryValueServiceTests(ITestOutputHelper output)
        {
            historyValuesService = new();
            Console.SetOut(new ConsoleWriter(output));
        }

        private async ValueTask BuildProjectAndCreateInstanceAsync()
        {
            wipProject = CreateWipProject(TDengineHistoryStorage.Prefix);
            runnableProject = await Project.BuildAsync(wipProject);
            runnableProject.CreateInstance(ClassName, InstanceName);
        }

        [Fact]
        public async Task GivenInstanceWithHistory_WhenGettingHistoryRawValues_ThenHistoryRawRowsAreReturned()
        {
            // Given
            await BuildProjectAndCreateInstanceAsync();
            DateTime timestamp = DateTime.UtcNow;
            runnableProject!.SetInstanceValue<int>(InstanceName, ValueFieldName, 123456, timestamp);
            HistoryRawResultModel expectedHistoryResult = new([new HistoryFieldModel(ValueFieldName, "System.Int32", 0)],
                [new HistoryRawRowModel(timestamp, QualityLevel.Good, [123456])]);

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

        [Fact]
        public async Task GivenInstanceWithHistory_WhenGettingHistoryValues_ThenHistoryRowsAreReturned()
        {
            // Given
            await BuildProjectAndCreateInstanceAsync();
            DateTime timestamp = DateTime.UtcNow;
            runnableProject!.SetInstanceValue<int>(InstanceName, ValueFieldName, 123456, timestamp);
            HistoryResultModel expectedHistoryResult = new([new HistoryFieldModel(ValueFieldName, "System.Int32", 0)],
                [new HistoryRowModel(timestamp, QualityLevel.Good, [new IntFieldValueModel(123456, null, QualityLevel.Good, timestamp)])]);

            // Act
            HistoryRequestModel request = new(timestamp.AddSeconds(-1), DateTime.Now, [ValueFieldName]);
            HistoryResultModel historyResult = historyValuesService.GetInstanceHistoryValues(runnableProject.GetId(), InstanceName, request);

            // Assert
            // This doesn' work, as comparison of the object values use Object Equals().
            historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
        }

        [Fact]
        public async Task GivenInstanceWithHistory_WhenGettingHistoryRawStatistics_ThenHistoryRawRowsAreReturned()
        {
            // Given
            await BuildProjectAndCreateInstanceAsync();
            DateTime timestamp = DateTime.UtcNow.Date;
            runnableProject!.SetInstanceValue<int>(InstanceName, ValueFieldName, 123456, timestamp);
            HistoryStatisticsRawResultModel expectedHistoryResult = new([new HistoryStatisticResultFieldModel(ValueFieldName, "System.Int32", 0, HistoryStatFunction.AVG)],
                [new HistoryStatisticsRawRowModel(timestamp, timestamp, timestamp.AddHours(1), TimeSpan.FromHours(1), QualityLevel.Good, [123456])]);

            // Act
            HistoryStatisticsRequestModel request = new(timestamp, timestamp.AddHours(1), TimeSpan.FromHours(1), FillMode.PREV,
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

        [Fact]
        public async Task GivenInstanceWithHistory_WhenGettingHistoryStatistics_ThenHistoryRowsAreReturned()
        {
            // Given
            await BuildProjectAndCreateInstanceAsync();
            DateTime timestamp = DateTime.UtcNow.Date;
            runnableProject!.SetInstanceValue<int>(InstanceName, ValueFieldName, 123456, timestamp);
            HistoryStatisticsResultModel expectedHistoryResult = new([new HistoryStatisticResultFieldModel(ValueFieldName, "System.Int32", 0, HistoryStatFunction.AVG)],
                [new HistoryStatisticsRowModel(timestamp, timestamp, timestamp.AddHours(1), TimeSpan.FromHours(1), QualityLevel.Good,
                    [new IntFieldValueModel(123456, null, QualityLevel.Good, timestamp)])]);

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
