using NSubstitute;
using pva.Helpers.Extensions;
using pva.SuperV.Api.Services.History;
using pva.SuperV.Engine;
using pva.SuperV.Model.HistoryRetrieval;
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
        public async Task GivenInstanceWithHistoryValues_WhenGettingHistoryValues_ThenHistoryValuesAreReturned()
        {
            // GIVEN
            DateTime rowTimestamp = DateTime.Now;
            HistoryRawResultModel expectedHistoryResult = new([new HistoryFieldModel("Field1", "System.Int32", 0)],
                [new HistoryRawRowModel(rowTimestamp, null, null, null, QualityLevel.Good, [JsonSerializer.SerializeToElement(1)])]);
            HistoryRequestModel request = new(DateTime.Now.AddHours(-1), DateTime.Now, null, null, ["Field1"]);
            MockedHistoryValuesService.GetInstanceRawHistoryValues("Project", "Instance", Arg.Any<HistoryRequestModel>())
                .Returns(expectedHistoryResult with
                {
                    Rows =
                    [new HistoryRawRowModel(rowTimestamp, null, null, null, QualityLevel.Good, [1])]
                });

            // WHEN
            var result = await client.PostAsJsonAsync("/history-values/Project/Instance/raw", request);

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
                actualRow.StartTime.ShouldBe(expectedRow.StartTime);
                actualRow.EndTime.ShouldBe(expectedRow.EndTime);
                actualRow.Duration.ShouldBe(expectedRow.Duration);
                actualRow.FieldValues.Count.ShouldBe(expectedRow.FieldValues.Count);
                for (int fieldIndex = 0; fieldIndex < actualRow.FieldValues.Count; fieldIndex++)
                {
                    JsonElement actualValue = (JsonElement)actualRow.FieldValues[fieldIndex];
                    JsonElement expectedValue = (JsonElement)expectedRow.FieldValues[fieldIndex];
                    Assert.True(actualValue.IsEqualTo(expectedValue));
                }
            }

        }
    }
}
