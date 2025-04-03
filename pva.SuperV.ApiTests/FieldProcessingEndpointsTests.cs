using NSubstitute;
using NSubstitute.ExceptionExtensions;
using pva.SuperV.Api.Exceptions;
using pva.SuperV.Api.Services.FieldProcessings;
using pva.SuperV.Engine.Exceptions;
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
        private IFieldProcessingService MockedFieldProcessingService { get => application.MockedFieldProcessingService!; }

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
        public async Task WhenGettingFieldProcessingsOnUnknownField_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedFieldProcessingService.GetFieldProcessings("Project1", "Class1", "UnknownFieldName")
                .Throws<UnknownEntityException>();

            // WHEN
            var response = await client.GetAsync("/field-processings/Project1/Class1/UnknownFieldName");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
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
        public async Task WhenGettingUnknownFieldProcessing_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedFieldProcessingService.GetFieldProcessing("Project1", "Class1", "TrigerringFieldName", "UnknownFieldProcessing")
                .Throws<UnknownEntityException>();

            // WHEN
            var response = await client.GetAsync($"/field-processings/Project1/Class1/TrigerringFieldName/UnknownFieldProcessing");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
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
        public async Task WhenCreatingFieldProcessingOnUnknownField_ThenNotFoundIsReturned()
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
            MockedFieldProcessingService.CreateFieldProcessing("Project1", "Class1", "UnknownFieldName", Arg.Any<FieldValueProcessingModel>())
                .Throws<UnknownEntityException>();

            // WHEN
            var response = await client.PostAsJsonAsync("/field-processings/Project1/Class1/UnknownFieldName", expectedFieldProcessing);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenCreatingFieldProcessingOnNonWipProject_ThenBadRequestIsReturned()
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
            MockedFieldProcessingService.CreateFieldProcessing("RunnableProject", "Class1", "TrigerringFieldName", Arg.Any<FieldValueProcessingModel>())
                .Throws<NonWipProjectException>();

            // WHEN
            var response = await client.PostAsJsonAsync("/field-processings/RunnableProject/Class1/TrigerringFieldName", expectedFieldProcessing);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenWipProject_WhenUpdatingFieldProcessing_ThenFieldProcessingIsUpdated()
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
            MockedFieldProcessingService.UpdateFieldProcessing("Project1", "Class1", "TrigerringFieldName", expectedFieldProcessing.Name, Arg.Any<FieldValueProcessingModel>())
                .Returns(expectedFieldProcessing);

            // WHEN
            var response = await client.PutAsJsonAsync($"/field-processings/Project1/Class1/TrigerringFieldName/{expectedFieldProcessing.Name}", expectedFieldProcessing);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            FieldValueProcessingModel? createdFieldProcessing = await response.Content.ReadFromJsonAsync<FieldValueProcessingModel>();
            createdFieldProcessing.ShouldBeEquivalentTo(expectedFieldProcessing);
        }

        [Fact]
        public async Task WhenUpdatingFieldProcessingOnUnknownField_ThenNotFoundIsReturned()
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
            MockedFieldProcessingService.UpdateFieldProcessing("Project1", "Class1", "UnknownFieldName", expectedFieldProcessing.Name, Arg.Any<FieldValueProcessingModel>())
                .Throws<UnknownEntityException>();

            // WHEN
            var response = await client.PutAsJsonAsync($"/field-processings/Project1/Class1/UnknownFieldName/{expectedFieldProcessing.Name}", expectedFieldProcessing);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenUpdatingFieldProcessingOnNonWipProject_ThenBadRequestIsReturned()
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
            MockedFieldProcessingService.UpdateFieldProcessing("RunnableProject", "Class1", "TrigerringFieldName", expectedFieldProcessing.Name, Arg.Any<FieldValueProcessingModel>())
                .Throws<NonWipProjectException>();

            // WHEN
            var response = await client.PutAsJsonAsync($"/field-processings/RunnableProject/Class1/TrigerringFieldName/{expectedFieldProcessing.Name}", expectedFieldProcessing);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenWipProject_WhenDeletingFieldProcessing_ThenFieldProcessingIsDeleted()
        {
            // GIVEN

            // WHEN
            var response = await client.DeleteAsync($"/field-processings/Project1/Class1/TrigerringFieldName/FieldProcessingName");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task WhenDeletingUnknownFieldProcessing_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedFieldProcessingService.When(fake => fake.DeleteFieldProcessing("Project1", "Class1", "TrigerringFieldName", "UnknownFieldProcessingName"))
                .Do(call => { throw new UnknownEntityException(); });

            // WHEN
            var response = await client.DeleteAsync($"/field-processings/Project1/Class1/TrigerringFieldName/UnknownFieldProcessingName");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenDeletingFieldProcessingOnNonWipProject_ThenBadRequestIsReturned()
        {
            // GIVEN
            MockedFieldProcessingService.When(fake => fake.DeleteFieldProcessing("RunnableProject", "Class1", "TrigerringFieldName", "FieldProcessingName"))
                .Do(call => { throw new NonWipProjectException(); });

            // WHEN
            var response = await client.DeleteAsync($"/field-processings/RunnableProject/Class1/TrigerringFieldName/FieldProcessingName");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
