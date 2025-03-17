using NSubstitute;
using pva.Helpers.Extensions;
using pva.SuperV.Api.Services.History;
using pva.SuperV.Engine;
using pva.SuperV.Model.HistoryRetrieval;
using pva.SuperV.Model.Instances;
using Shouldly;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit.Abstractions;

namespace pva.SuperV.ApiTests
{
    public class HistoryValuesEndpointsTests
    {
        public class ConsoleWriter(ITestOutputHelper output) : StringWriter
        {
            public override void WriteLine(string? value) => output.WriteLine(value);
        }

        private readonly TestProjectApplication application;
        private readonly HttpClient client;
        private IHistoryValuesService MockedHistoryValuesService { get => application.MockedHistoryValueService!; }

        public HistoryValuesEndpointsTests(ITestOutputHelper output)
        {
            application = new();
            client = application.CreateClient();
            Console.SetOut(new ConsoleWriter(output));
        }

        [Fact]
        public async Task GivenInstanceWithHistoryValues_WhenGettingHistoryRawValues_ThenHistoryRawValuesAreReturned()
        {
            // GIVEN
            DateTime rowTimestamp = DateTime.Now;
            HistoryRawResultModel expectedHistoryResult = new([new HistoryFieldModel("Field1", "System.Int32", 0)],
                [new HistoryRawRowModel(rowTimestamp, QualityLevel.Good, [JsonSerializer.SerializeToElement(1)])]);
            HistoryRequestModel request = new(DateTime.Now.AddHours(-1), DateTime.Now, ["Field1"]);
            MockedHistoryValuesService.GetInstanceRawHistoryValues("Project", "Instance", Arg.Any<HistoryRequestModel>())
                .Returns(expectedHistoryResult with
                {
                    Rows =
                    [new HistoryRawRowModel(rowTimestamp, QualityLevel.Good, [1])]
                });

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/Instance/values/raw", request);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            HistoryRawResultModel? historyResult = await result.Content.ReadFromJsonAsync<HistoryRawResultModel>();
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
                actualRow.FieldValues.Count.ShouldBe(expectedRow.FieldValues.Count);
                for (int fieldIndex = 0; fieldIndex < actualRow.FieldValues.Count; fieldIndex++)
                {
                    JsonElement actualValue = (JsonElement)actualRow.FieldValues[fieldIndex];
                    JsonElement expectedValue = (JsonElement)expectedRow.FieldValues[fieldIndex];
                    Assert.True(actualValue.IsEqualTo(expectedValue));
                }
            }

        }

        [Fact]
        public async Task GivenInstanceWithHistoryValues_WhenGettingHistoryValues_ThenHistoryValuesAreReturned()
        {
            // GIVEN
            DateTime rowTimestamp = DateTime.Now;
            HistoryResultModel expectedHistoryResult = new([new HistoryFieldModel("Field1", "System.Int32", 0)],
                [new HistoryRowModel(rowTimestamp, QualityLevel.Good, [new IntFieldValueModel(1, null, QualityLevel.Good, rowTimestamp)])]);
            HistoryRequestModel request = new(DateTime.Now.AddHours(-1), DateTime.Now, ["Field1"]);
            MockedHistoryValuesService.GetInstanceHistoryValues("Project", "Instance", Arg.Any<HistoryRequestModel>())
                .Returns(expectedHistoryResult);

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/Instance/values", request);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            HistoryResultModel? historyResult = await result.Content.ReadFromJsonAsync<HistoryResultModel>();
            historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
        }

        [Fact]
        public async Task GivenInstanceWithHistoryValues_WhenGettingHistoryStatistics_ThenHistoryRawStatisticsAreReturned()
        {
            // GIVEN
            DateTime rowTimestamp = DateTime.Now;
            HistoryStatisticsRawResultModel expectedHistoryResult = new([new HistoryStatisticResultFieldModel("Field1", "System.Int32", 0, Engine.HistoryRetrieval.HistoryStatFunction.AVG)],
                [new HistoryStatisticsRawRowModel(rowTimestamp, rowTimestamp, rowTimestamp.AddHours(1), TimeSpan.FromHours(1), QualityLevel.Good, [JsonSerializer.SerializeToElement(1)])]);
            HistoryStatisticsRequestModel request = new(DateTime.Now.AddHours(-1), DateTime.Now, TimeSpan.FromHours(1), Engine.HistoryRetrieval.FillMode.PREV,
                [new HistoryStatisticFieldModel("Field1", Engine.HistoryRetrieval.HistoryStatFunction.AVG)]);
            MockedHistoryValuesService.GetInstanceRawHistoryStatistics("Project", "Instance", Arg.Any<HistoryStatisticsRequestModel>())
                .Returns(expectedHistoryResult with
                {
                    Rows =
                    [new HistoryStatisticsRawRowModel(rowTimestamp, rowTimestamp, rowTimestamp.AddHours(1), TimeSpan.FromHours(1), QualityLevel.Good, [1])]
                });

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/Instance/statistics/raw", request);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            HistoryStatisticsRawResultModel? historyResult = await result.Content.ReadFromJsonAsync<HistoryStatisticsRawResultModel>();
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
                actualRow.FieldValues.Count.ShouldBe(expectedRow.FieldValues.Count);
                for (int fieldIndex = 0; fieldIndex < actualRow.FieldValues.Count; fieldIndex++)
                {
                    JsonElement actualValue = (JsonElement)actualRow.FieldValues[fieldIndex];
                    JsonElement expectedValue = (JsonElement)expectedRow.FieldValues[fieldIndex];
                    Assert.True(actualValue.IsEqualTo(expectedValue));
                }
            }

        }

        [Fact]
        public async Task GivenInstanceWithHistoryValues_WhenGettingHistoryStatistics_ThenHistoryStatisticsAreReturned()
        {
            // GIVEN
            DateTime rowTimestamp = DateTime.Now;
            HistoryStatisticsResultModel expectedHistoryResult = new([new HistoryStatisticResultFieldModel("Field1", "System.Int32", 0, Engine.HistoryRetrieval.HistoryStatFunction.AVG)],
                [new HistoryStatisticsRowModel(rowTimestamp, rowTimestamp, rowTimestamp.AddHours(1), TimeSpan.FromHours(1), QualityLevel.Good, [new IntFieldValueModel(1, null, QualityLevel.Good, rowTimestamp)])]);
            HistoryStatisticsRequestModel request = new(DateTime.Now.AddHours(-1), DateTime.Now, TimeSpan.FromHours(1), Engine.HistoryRetrieval.FillMode.PREV,
                [new HistoryStatisticFieldModel("Field1", Engine.HistoryRetrieval.HistoryStatFunction.AVG)]);
            MockedHistoryValuesService.GetInstanceHistoryStatistics("Project", "Instance", Arg.Any<HistoryStatisticsRequestModel>())
                .Returns(expectedHistoryResult);

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/Instance/statistics", request);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            HistoryStatisticsResultModel? historyResult = await result.Content.ReadFromJsonAsync<HistoryStatisticsResultModel>();
            historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
        }
    }
}
