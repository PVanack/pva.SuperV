using NSubstitute;
using NSubstitute.ExceptionExtensions;
using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;
using Shouldly;
using System.Net.Http.Json;

namespace pva.SuperV.ApiTests
{

    public class FieldProcessingEndpointsTests
    {
        public class ConsoleWriter(ITestOutputHelper output) : StringWriter
        {
            public override void WriteLine(string? value) => output.WriteLine(value!);
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
            MockedFieldProcessingService.GetFieldProcessingsAsync("Project1", "Class1", "TrigerringFieldName")
                .Returns(expectedFieldProcessings);

            // WHEN
            var response = await client.GetAsync("/field-processings/Project1/Class1/TrigerringFieldName", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            FieldValueProcessingModel[]? fieldProcessings = await response.Content.ReadFromJsonAsync<FieldValueProcessingModel[]>(cancellationToken: TestContext.Current.CancellationToken);
            fieldProcessings.ShouldBeEquivalentTo(expectedFieldProcessings.ToArray());
        }

        [Fact]
        public async Task WhenGettingFieldProcessingsOnUnknownField_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedFieldProcessingService.GetFieldProcessingsAsync("Project1", "Class1", "UnknownFieldName")
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.GetAsync("/field-processings/Project1/Class1/UnknownFieldName", TestContext.Current.CancellationToken);

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

            MockedFieldProcessingService.GetFieldProcessingAsync("Project1", "Class1", "TrigerringFieldName", expectedFieldProcessing.Name)
                .Returns(expectedFieldProcessing);

            // WHEN
            var response = await client.GetAsync($"/field-processings/Project1/Class1/TrigerringFieldName/{expectedFieldProcessing.Name}", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            FieldValueProcessingModel? retrievedFieldProcessing = await response.Content.ReadFromJsonAsync<FieldValueProcessingModel>(cancellationToken: TestContext.Current.CancellationToken);
            retrievedFieldProcessing.ShouldBeEquivalentTo(expectedFieldProcessing);
        }

        [Fact]
        public async Task WhenGettingUnknownFieldProcessing_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedFieldProcessingService.GetFieldProcessingAsync("Project1", "Class1", "TrigerringFieldName", "UnknownFieldProcessing")
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.GetAsync("/field-processings/Project1/Class1/TrigerringFieldName/UnknownFieldProcessing", TestContext.Current.CancellationToken);

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
            MockedFieldProcessingService.CreateFieldProcessingAsync("Project1", "Class1", "TrigerringFieldName", Arg.Any<FieldValueProcessingModel>())
                .Returns(expectedFieldProcessing);

            // WHEN
            var response = await client.PostAsJsonAsync("/field-processings/Project1/Class1/TrigerringFieldName", expectedFieldProcessing, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            FieldValueProcessingModel? createdFieldProcessing = await response.Content.ReadFromJsonAsync<FieldValueProcessingModel>(cancellationToken: TestContext.Current.CancellationToken);
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
            MockedFieldProcessingService.CreateFieldProcessingAsync("Project1", "Class1", "UnknownFieldName", Arg.Any<FieldValueProcessingModel>())
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.PostAsJsonAsync("/field-processings/Project1/Class1/UnknownFieldName", expectedFieldProcessing, cancellationToken: TestContext.Current.CancellationToken);

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
            MockedFieldProcessingService.CreateFieldProcessingAsync("RunnableProject", "Class1", "TrigerringFieldName", Arg.Any<FieldValueProcessingModel>())
                .ThrowsAsync<NonWipProjectException>();

            // WHEN
            var response = await client.PostAsJsonAsync("/field-processings/RunnableProject/Class1/TrigerringFieldName", expectedFieldProcessing, cancellationToken: TestContext.Current.CancellationToken);

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
            MockedFieldProcessingService.UpdateFieldProcessingAsync("Project1", "Class1", "TrigerringFieldName", expectedFieldProcessing.Name, Arg.Any<FieldValueProcessingModel>())
                .Returns(expectedFieldProcessing);

            // WHEN
            var response = await client.PutAsJsonAsync($"/field-processings/Project1/Class1/TrigerringFieldName/{expectedFieldProcessing.Name}", expectedFieldProcessing, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            FieldValueProcessingModel? createdFieldProcessing = await response.Content.ReadFromJsonAsync<FieldValueProcessingModel>(cancellationToken: TestContext.Current.CancellationToken);
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
            MockedFieldProcessingService.UpdateFieldProcessingAsync("Project1", "Class1", "UnknownFieldName", expectedFieldProcessing.Name, Arg.Any<FieldValueProcessingModel>())
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.PutAsJsonAsync($"/field-processings/Project1/Class1/UnknownFieldName/{expectedFieldProcessing.Name}", expectedFieldProcessing, cancellationToken: TestContext.Current.CancellationToken);

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
            MockedFieldProcessingService.UpdateFieldProcessingAsync("RunnableProject", "Class1", "TrigerringFieldName", expectedFieldProcessing.Name, Arg.Any<FieldValueProcessingModel>())
                .ThrowsAsync<NonWipProjectException>();

            // WHEN
            var response = await client.PutAsJsonAsync($"/field-processings/RunnableProject/Class1/TrigerringFieldName/{expectedFieldProcessing.Name}", expectedFieldProcessing, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenWipProject_WhenDeletingFieldProcessing_ThenFieldProcessingIsDeleted()
        {
            // GIVEN

            // WHEN
            var response = await client.DeleteAsync("/field-processings/Project1/Class1/TrigerringFieldName/FieldProcessingName", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task WhenDeletingUnknownFieldProcessing_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedFieldProcessingService.When(async (fake) => await fake.DeleteFieldProcessingAsync("Project1", "Class1", "TrigerringFieldName", "UnknownFieldProcessingName"))
                .Do(_ => throw new UnknownEntityException());

            // WHEN
            var response = await client.DeleteAsync("/field-processings/Project1/Class1/TrigerringFieldName/UnknownFieldProcessingName", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenDeletingFieldProcessingOnNonWipProject_ThenBadRequestIsReturned()
        {
            // GIVEN
            MockedFieldProcessingService.When(async (fake) => await fake.DeleteFieldProcessingAsync("RunnableProject", "Class1", "TrigerringFieldName", "FieldProcessingName"))
                .Do(_ => throw new NonWipProjectException());

            // WHEN
            var response = await client.DeleteAsync("/field-processings/RunnableProject/Class1/TrigerringFieldName/FieldProcessingName", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
