using NSubstitute;
using pva.SuperV.Api.Services.FieldProcessings;
using pva.SuperV.Model.FieldProcessings;
using Shouldly;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace pva.SuperV.ApiTests
{

    public class FieldProcessingEndpointsTests
    {
        public class ConsoleWriter(ITestOutputHelper output) : StringWriter
        {
            public override void WriteLine(string? value) => output.WriteLine(value);
        }

        private readonly TestProjectApplication application;
        private readonly HttpClient client;
        private IFieldProcessingService MockedFieldProcessingService { get => application.MockFieldProcessingService!; }

        public FieldProcessingEndpointsTests(ITestOutputHelper output)
        {
            application = new();
            client = application.CreateClient();
            Console.SetOut(new ConsoleWriter(output));
        }

        [Fact]
        public async Task GivenExistingProcessingsOnField_WhenGettingFieldProcessings_ThenFieldProcessingsAreReturned()
        {
            // GIVEN
            List<FieldValueProcessingModel> expectedFieldProcessings =
                [
                    new AlarmStateProcessingModel("AlarmStateProcessing",
                        "TrigerringFieldName",
                        null,
                        "HighLimitFieldName",
                        "LowLimitFieldName",
                        null,
                        null,
                        "AlarmStateFieldName",
                        null),
                    new HistorizationProcessingModel("HistorizationProcessing",
                        "TrigerringFieldName",
                        "HistoryRepositoryName",
                        null,
                        ["Field1"])
            ];
            MockedFieldProcessingService.GetFieldProcessings("Project1", "Class1", "TrigerringFieldName")
                .Returns(expectedFieldProcessings);

            // WHEN
            var response = await client.GetAsync("/field-processings/Project1/Class1/TrigerringFieldName");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            FieldValueProcessingModel[]? fieldProcessings = await response.Content.ReadFromJsonAsync<FieldValueProcessingModel[]>();
            fieldProcessings.ShouldBeEquivalentTo(expectedFieldProcessings.ToArray());
        }

        [Fact]
        public async Task GivenExistingFieldProcessingsOnField_WhenGettingFieldProcessing_ThenFieldProcessingIsReturned()
        {
            // GIVEN
            AlarmStateProcessingModel expectedFieldProcessing = new("AlarmStateProcessing",
                        "TrigerringFieldName",
                        null,
                        "HighLimitFieldName",
                        "LowLimitFieldName",
                        null,
                        null,
                        "AlarmStateFieldName",
                        null);

            MockedFieldProcessingService.GetFieldProcessing("Project1", "Class1", "TrigerringFieldName", expectedFieldProcessing.Name)
                .Returns(expectedFieldProcessing);

            // WHEN
            var response = await client.GetAsync($"/field-processings/Project1/Class1/TrigerringFieldName/{expectedFieldProcessing.Name}");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            FieldValueProcessingModel? retrievedFieldProcessing = await response.Content.ReadFromJsonAsync<FieldValueProcessingModel>();
            retrievedFieldProcessing.ShouldBeEquivalentTo(expectedFieldProcessing);
        }

        [Fact]
        public async Task GivenWipProject_WhenCreatingFieldProcessing_ThenFieldProcessingIsCreated()
        {
            // GIVEN
            FieldValueProcessingModel expectedFieldProcessing = new AlarmStateProcessingModel("AlarmStateProcessing",
                        "TrigerringFieldName",
                        null,
                        "HighLimitFieldName",
                        "LowLimitFieldName",
                        null,
                        null,
                        "AlarmStateFieldName",
                        null);
            MockedFieldProcessingService.CreateFieldProcessing("Project1", "Class1", "TrigerringFieldName", Arg.Any<FieldValueProcessingModel>())
                .Returns(expectedFieldProcessing);

            // WHEN
            var response = await client.PostAsJsonAsync("/field-processings/Project1/Class1/TrigerringFieldName", expectedFieldProcessing);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            FieldValueProcessingModel? createdFieldProcessing = await response.Content.ReadFromJsonAsync<FieldValueProcessingModel>();
            createdFieldProcessing.ShouldBeEquivalentTo(expectedFieldProcessing);
        }

        [Fact]
        public async Task GivenWipProject_WhenDeletingFieldProcessing_ThenFieldProcessingIsDeleted()
        {
            // GIVEN
            FieldValueProcessingModel expectedFieldProcessing = new AlarmStateProcessingModel("AlarmStateProcessing",
                        "TrigerringFieldName",
                        null,
                        "HighLimitFieldName",
                        "LowLimitFieldName",
                        null,
                        null,
                        "AlarmStateFieldName",
                        null);

            // WHEN
            var response = await client.DeleteAsync($"/field-processings/Project1/Class1/TrigerringFieldName/{expectedFieldProcessing.Name}");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
        }
    }
}
