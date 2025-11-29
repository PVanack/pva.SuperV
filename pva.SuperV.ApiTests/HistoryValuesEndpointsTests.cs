using NSubstitute;
using NSubstitute.ExceptionExtensions;
using pva.Helpers.Extensions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.HistoryRetrieval;
using pva.SuperV.Model.Instances;
using pva.SuperV.Model.Services;
using Shouldly;
using System.Net.Http.Json;
using System.Text.Json;

namespace pva.SuperV.ApiTests
{
    public class HistoryValuesEndpointsTests
    {
        public class ConsoleWriter(ITestOutputHelper output) : StringWriter
        {
            public override void WriteLine(string? value) => output.WriteLine(value!);
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
            MockedHistoryValuesService.GetInstanceRawHistoryValuesAsync("Project", "Instance", Arg.Any<HistoryRequestModel>())
                .Returns(expectedHistoryResult with
                {
                    Rows =
                    [new HistoryRawRowModel(rowTimestamp, QualityLevel.Good, [1])]
                });

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/Instance/values/raw", request, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            HistoryRawResultModel? historyResult = await result.Content.ReadFromJsonAsync<HistoryRawResultModel>(cancellationToken: TestContext.Current.CancellationToken);
            CheckHistoryResult(expectedHistoryResult, historyResult);
        }

        private static void CheckHistoryResult(HistoryRawResultModel expectedHistoryResult, HistoryRawResultModel? historyResult)
        {
            // This doesn' work, as comparison of the object values use Object Equals().
            //historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
            // As a workaround, we compare each element :-(
            historyResult.ShouldNotBeNull();
            historyResult.Header.ShouldBeEquivalentTo(expectedHistoryResult.Header);
            historyResult.Rows.Count.ShouldBe(expectedHistoryResult.Rows.Count);
            for (int rowIndex = 0; rowIndex < expectedHistoryResult.Rows.Count; rowIndex++)
            {
                var actualRow = historyResult.Rows[rowIndex];
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
        public async Task WhenGettingHistoryRawValuesOnUnknownInstance_ThenNotFoundIsReturned()
        {
            // GIVEN
            HistoryRequestModel request = new(DateTime.Now.AddHours(-1), DateTime.Now, ["Field1"]);
            MockedHistoryValuesService.GetInstanceRawHistoryValuesAsync("Project", "UnknownInstance", Arg.Any<HistoryRequestModel>())
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/UnknownInstance/values/raw", request, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenGettingHistoryRawValuesWithStartTimeEqualToEndTime_ThenBadRequestReturned()
        {
            // GIVEN
            DateTime endTime = DateTime.UtcNow;
            HistoryRequestModel request = new(endTime, endTime, ["Field1"]);
            MockedHistoryValuesService.GetInstanceRawHistoryValuesAsync("Project", "Instance", Arg.Any<HistoryRequestModel>())
                .ThrowsAsync<BadHistoryStartTimeException>();

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/Instance/values/raw", request, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenInstanceWithHistoryValues_WhenGettingHistoryValues_ThenHistoryValuesAreReturned()
        {
            // GIVEN
            DateTime rowTimestamp = DateTime.Now;
            HistoryResultModel expectedHistoryResult = new([new HistoryFieldModel("Field1", "System.Int32", 0)],
                [new HistoryRowModel(rowTimestamp, QualityLevel.Good, [new IntFieldValueModel(1, null, QualityLevel.Good, rowTimestamp)])]);
            HistoryRequestModel request = new(DateTime.Now.AddHours(-1), DateTime.Now, ["Field1"]);
            MockedHistoryValuesService.GetInstanceHistoryValuesAsync("Project", "Instance", Arg.Any<HistoryRequestModel>())
                .Returns(expectedHistoryResult);

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/Instance/values", request, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            HistoryResultModel? historyResult = await result.Content.ReadFromJsonAsync<HistoryResultModel>(cancellationToken: TestContext.Current.CancellationToken);
            historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
        }

        [Fact]
        public async Task WhenGettingHistoryValuesOnUnknownInstance_ThenNotFoundIsReturned()
        {
            // GIVEN
            HistoryRequestModel request = new(DateTime.Now.AddHours(-1), DateTime.Now, ["Field1"]);
            MockedHistoryValuesService.GetInstanceHistoryValuesAsync("Project", "UnknownInstance", Arg.Any<HistoryRequestModel>())
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/UnknownInstance/values", request, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenGettingHistoryValuesWithStartTimeEqualToEndTime_ThenBadRequestReturned()
        {
            // GIVEN
            DateTime endTime = DateTime.Now;
            HistoryRequestModel request = new(endTime, endTime, ["Field1"]);
            MockedHistoryValuesService.GetInstanceHistoryValuesAsync("Project", "Instance", Arg.Any<HistoryRequestModel>())
                .ThrowsAsync<BadHistoryStartTimeException>();

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/Instance/values", request, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenInstanceWithHistoryValues_WhenGettingHistoryRawStatistics_ThenHistoryRawStatisticsAreReturned()
        {
            // GIVEN
            DateTime rowTimestamp = DateTime.Now;
            HistoryStatisticsRawResultModel expectedHistoryResult = new([new HistoryStatisticResultFieldModel("Field1", "System.Int32", 0, Engine.HistoryRetrieval.HistoryStatFunction.AVG)],
                [new HistoryStatisticsRawRowModel(rowTimestamp, rowTimestamp, rowTimestamp.AddHours(1), TimeSpan.FromHours(1), QualityLevel.Good, [JsonSerializer.SerializeToElement(1)])]);
            HistoryStatisticsRequestModel request = new(DateTime.Now.AddHours(-1), DateTime.Now, TimeSpan.FromHours(1), Engine.HistoryRetrieval.FillMode.PREV,
                [new HistoryStatisticFieldModel("Field1", Engine.HistoryRetrieval.HistoryStatFunction.AVG)]);
            MockedHistoryValuesService.GetInstanceRawHistoryStatisticsAsync("Project", "Instance", Arg.Any<HistoryStatisticsRequestModel>())
                .Returns(expectedHistoryResult with
                {
                    Rows =
                    [new HistoryStatisticsRawRowModel(rowTimestamp, rowTimestamp, rowTimestamp.AddHours(1), TimeSpan.FromHours(1), QualityLevel.Good, [1])]
                });

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/Instance/statistics/raw", request, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            HistoryStatisticsRawResultModel? historyResult = await result.Content.ReadFromJsonAsync<HistoryStatisticsRawResultModel>(cancellationToken: TestContext.Current.CancellationToken);
            CheckHistoryStatisticsResult(expectedHistoryResult, historyResult);
        }

        private static void CheckHistoryStatisticsResult(HistoryStatisticsRawResultModel expectedHistoryResult, HistoryStatisticsRawResultModel? historyResult)
        {
            // This doesn' work, as comparison of the object values use Object Equals().
            //historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
            // As a workaround, we compare each element :-(
            historyResult.ShouldNotBeNull();
            historyResult.Header.ShouldBeEquivalentTo(expectedHistoryResult.Header);
            historyResult.Rows.Count.ShouldBe(expectedHistoryResult.Rows.Count);
            for (int rowIndex = 0; rowIndex < expectedHistoryResult.Rows.Count; rowIndex++)
            {
                var actualRow = historyResult.Rows[rowIndex];
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
        public async Task WhenGettingHistoryRawStatisticsOnUnknownInstance_ThenNotFoundIsReturned()
        {
            // GIVEN
            HistoryStatisticsRequestModel request = new(DateTime.Now.AddHours(-1), DateTime.Now, TimeSpan.FromHours(1), Engine.HistoryRetrieval.FillMode.PREV,
                [new HistoryStatisticFieldModel("Field1", Engine.HistoryRetrieval.HistoryStatFunction.AVG)]);
            MockedHistoryValuesService.GetInstanceRawHistoryStatisticsAsync("Project", "UnknownInstance", Arg.Any<HistoryStatisticsRequestModel>())
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/UnknownInstance/statistics/raw", request, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenGettingHistoryRawStatisticsWithStartTimeEqualToEndTime_ThenBadRequestReturned()
        {
            // GIVEN
            DateTime endTime = DateTime.UtcNow;
            HistoryStatisticsRequestModel request = new(endTime, endTime, TimeSpan.FromHours(1), Engine.HistoryRetrieval.FillMode.PREV,
                [new HistoryStatisticFieldModel("Field1", Engine.HistoryRetrieval.HistoryStatFunction.AVG)]);
            MockedHistoryValuesService.GetInstanceRawHistoryStatisticsAsync("Project", "Instance", Arg.Any<HistoryStatisticsRequestModel>())
                .ThrowsAsync<BadHistoryStartTimeException>();

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/Instance/statistics/raw", request, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task WhenGettingHistoryRawStatisticsWithIntervalToStartTimeMinusEndTime_ThenBadRequestReturned()
        {
            // GIVEN
            DateTime endTime = DateTime.UtcNow;
            DateTime startTime = endTime.AddHours(-1);
            HistoryStatisticsRequestModel request = new(startTime, endTime, TimeSpan.FromDays(1), Engine.HistoryRetrieval.FillMode.PREV,
                [new HistoryStatisticFieldModel("Field1", Engine.HistoryRetrieval.HistoryStatFunction.AVG)]);
            MockedHistoryValuesService.GetInstanceRawHistoryStatisticsAsync("Project", "Instance", Arg.Any<HistoryStatisticsRequestModel>())
                .ThrowsAsync<BadHistoryIntervalException>();

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/Instance/statistics/raw", request, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
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
            MockedHistoryValuesService.GetInstanceHistoryStatisticsAsync("Project", "Instance", Arg.Any<HistoryStatisticsRequestModel>())
                .Returns(expectedHistoryResult);

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/Instance/statistics", request, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            HistoryStatisticsResultModel? historyResult = await result.Content.ReadFromJsonAsync<HistoryStatisticsResultModel>(cancellationToken: TestContext.Current.CancellationToken);
            historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
        }

        [Fact]
        public async Task WhenGettingHistoryStatisticsOnUnknownInstance_ThenNotFoundIsReturned()
        {
            // GIVEN
            HistoryStatisticsRequestModel request = new(DateTime.Now.AddHours(-1), DateTime.Now, TimeSpan.FromHours(1), Engine.HistoryRetrieval.FillMode.PREV,
                [new HistoryStatisticFieldModel("Field1", Engine.HistoryRetrieval.HistoryStatFunction.AVG)]);
            MockedHistoryValuesService.GetInstanceHistoryStatisticsAsync("Project", "UnknownInstance", Arg.Any<HistoryStatisticsRequestModel>())
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/UnknownInstance/statistics", request, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenGettingHistoryStatisticsWithStartTimeEqualToEndTime_ThenBadRequestReturned()
        {
            // GIVEN
            DateTime endTime = DateTime.UtcNow;
            HistoryStatisticsRequestModel request = new(endTime, endTime, TimeSpan.FromHours(1), Engine.HistoryRetrieval.FillMode.PREV,
                [new HistoryStatisticFieldModel("Field1", Engine.HistoryRetrieval.HistoryStatFunction.AVG)]);
            MockedHistoryValuesService.GetInstanceHistoryStatisticsAsync("Project", "Instance", Arg.Any<HistoryStatisticsRequestModel>())
                .ThrowsAsync<BadHistoryStartTimeException>();

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/Instance/statistics", request, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task WhenGettingHistoryStatisticsWithIntervalToStartTimeMinusEndTime_ThenBadRequestReturned()
        {
            // GIVEN
            DateTime endTime = DateTime.UtcNow;
            DateTime startTime = endTime.AddHours(-1);
            HistoryStatisticsRequestModel request = new(startTime, endTime, TimeSpan.FromDays(1), Engine.HistoryRetrieval.FillMode.PREV,
                [new HistoryStatisticFieldModel("Field1", Engine.HistoryRetrieval.HistoryStatFunction.AVG)]);
            MockedHistoryValuesService.GetInstanceHistoryStatisticsAsync("Project", "Instance", Arg.Any<HistoryStatisticsRequestModel>())
                .ThrowsAsync<BadHistoryIntervalException>();

            // WHEN
            var result = await client.PostAsJsonAsync("/history/Project/Instance/statistics", request, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
