using NSubstitute;
using NSubstitute.ExceptionExtensions;
using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.Instances;
using pva.SuperV.Model.Services;
using Shouldly;
using System.Net.Http.Json;

namespace pva.SuperV.ApiTests
{
    public class InstancesEndpointsTests
    {
        public class ConsoleWriter(ITestOutputHelper output) : StringWriter
        {
            public override void WriteLine(string? value) => output.WriteLine(value!);
        }

        private readonly TestProjectApplication application;
        private readonly HttpClient client;
        private IInstanceService MockedInstanceService { get => application.MockedInstanceService!; }
        private IFieldValueService MockedFieldValueService { get => application.MockedFieldValueService!; }

        public InstancesEndpointsTests(ITestOutputHelper output)
        {
            application = new();
            client = application.CreateClient();
            Console.SetOut(new ConsoleWriter(output));
        }

        [Fact]
        public async Task GivenExistingInstancesInProject_WhenSearchingProjectInstances_ThenInstancesAreReturned()
        {
            // GIVEN
            InstancePagedSearchRequest search = new(1, 10, null, null, null);
            List<InstanceModel> expectedInstances = [new InstanceModel("Instance", "Class1", [])];
            MockedInstanceService.SearchInstancesAsync("Project1", search)
                .Returns(new PagedSearchResult<InstanceModel>(1, 10, expectedInstances.Count, expectedInstances));

            // WHEN
            var response = await client.PostAsJsonAsync("/instances/Project1/search", search, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            PagedSearchResult<InstanceModel>? projectInstances = await response.Content.ReadFromJsonAsync<PagedSearchResult<InstanceModel>>(cancellationToken: TestContext.Current.CancellationToken);
            projectInstances.ShouldNotBeNull();
            projectInstances.PageNumber.ShouldBe(1);
            projectInstances.PageSize.ShouldBe(10);
            projectInstances.Count.ShouldBe(expectedInstances.Count);
            projectInstances.Result.ShouldBeEquivalentTo(expectedInstances);
        }

        [Fact]
        public async Task GivenExistingInstancesInProject_WhenGettingProjectInstances_ThenInstancesAreReturned()
        {
            // GIVEN
            List<InstanceModel> expectedInstances =
                [
                new InstanceModel("Instance1", "Class1",
                    [
                        new FieldModel("Field1", typeof(int).ToString(),
                            new ShortFieldValueModel(1, null, Engine.QualityLevel.Good, DateTime.Now))
                    ])
                ];
            MockedInstanceService.GetInstancesAsync("Project1")
                .Returns(expectedInstances);

            // WHEN
            var response = await client.GetAsync("/instances/Project1", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            InstanceModel[]? projectInstances = await response.Content.ReadFromJsonAsync<InstanceModel[]>(cancellationToken: TestContext.Current.CancellationToken);
            projectInstances.ShouldBeEquivalentTo(expectedInstances.ToArray());
        }

        [Fact]
        public async Task WhenGettingUnknownProjectInstances_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedInstanceService.GetInstancesAsync("UnknownProject")
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.GetAsync("/instances/UnknownProject", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenExistingInstancesInProject_WhenGettingProjectInstance_ThenInstanceIsReturned()
        {
            // GIVEN
            InstanceModel expectedInstance =
                new("Instance1", "Class1",
                    [
                        new FieldModel("Field1", typeof(int).ToString(),
                        new ShortFieldValueModel(1, null, Engine.QualityLevel.Good, DateTime.Now))
                    ]);
            MockedInstanceService.GetInstanceAsync("Project1", expectedInstance.Name)
                .Returns(expectedInstance);

            // WHEN
            var response = await client.GetAsync($"/instances/Project1/{expectedInstance.Name}", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            InstanceModel? retrievedInstance = await response.Content.ReadFromJsonAsync<InstanceModel>(cancellationToken: TestContext.Current.CancellationToken);
            retrievedInstance.ShouldBeEquivalentTo(expectedInstance);
        }

        [Fact]
        public async Task WhenGettingProjectUnknownInstance_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedInstanceService.GetInstanceAsync("Project1", "UnknownInstance")
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.GetAsync("/instances/Project1/UnknownInstance", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenWipProject_WhenCreatingProjectInstanceWithFieldValues_ThenInstanceIsCreatedWithFieldValues()
        {
            // GIVEN
            InstanceModel expectedInstance =
                new("Instance1", "Class1",
                    [
                        new FieldModel("Field1", typeof(int).ToString(),
                        new ShortFieldValueModel(1, null, Engine.QualityLevel.Good, DateTime.Now))
                    ]);
            MockedInstanceService.CreateInstanceAsync("Project1", Arg.Any<InstanceModel>())
                .Returns(expectedInstance);

            // WHEN
            var response = await client.PostAsJsonAsync("/instances/Project1/", expectedInstance, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            InstanceModel? createdInstance = await response.Content.ReadFromJsonAsync<InstanceModel>(cancellationToken: TestContext.Current.CancellationToken);
            createdInstance.ShouldBeEquivalentTo(expectedInstance);
        }

        [Fact]
        public async Task WhenCreatingUnknownProjectInstance_ThenNotFoundIsReturned()
        {
            // GIVEN
            InstanceModel expectedInstance =
                new("Instance1", "Class1",
                    [
                        new FieldModel("Field1", typeof(int).ToString(),
                        new ShortFieldValueModel(1, null, Engine.QualityLevel.Good, DateTime.Now))
                    ]);
            MockedInstanceService.CreateInstanceAsync("UnknownProject", Arg.Any<InstanceModel>())
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.PostAsJsonAsync("/instances/UnknownProject/", expectedInstance, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenCreatingInstanceOnNonWipProject_ThenBadRequestIsReturned()
        {
            // GIVEN
            InstanceModel expectedInstance =
                new("Instance1", "Class1",
                    [
                        new FieldModel("Field1", typeof(int).ToString(),
                        new ShortFieldValueModel(1, null, Engine.QualityLevel.Good, DateTime.Now))
                    ]);
            MockedInstanceService.CreateInstanceAsync("RunnableProject", Arg.Any<InstanceModel>())
                .ThrowsAsync<NonWipProjectException>();

            // WHEN
            var response = await client.PostAsJsonAsync("/instances/RunnableProject/", expectedInstance, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenWipProject_WhenDeletingProjectInstance_ThenInstanceIsDeleted()
        {
            // GIVEN
            InstanceModel expectedInstance =
                new("Instance1", "Class1",
                    [
                        new FieldModel("Field1", typeof(int).ToString(),
                        new ShortFieldValueModel(1, null, Engine.QualityLevel.Good, DateTime.Now))
                    ]);

            // WHEN
            var response = await client.DeleteAsync($"/instances/Project1/{expectedInstance.Name}", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task WhenDeletingInstanceOnNonWipProject_ThenBadRequestIsReturned()
        {
            // GIVEN
            MockedInstanceService.When(async (fake) => await fake.DeleteInstanceAsync("Project", "UnknownInstance"))
                .Do(_ => throw new UnknownEntityException());

            // WHEN
            var response = await client.DeleteAsync("/instances/Project/UnknownInstance", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenDeletingProjectUnknownInstance_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedInstanceService.When(async (fake) => await fake.DeleteInstanceAsync("RunnableProject", "Instance"))
                .Do(_ => throw new NonWipProjectException());

            // WHEN
            var response = await client.DeleteAsync("/instances/RunnableProject/Instance", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenWipProject_WhenGettingProjectInstanceField_ThenFieldIsReturned()
        {
            // GIVEN
            FieldModel expectedField = new("Field1", typeof(int).ToString(),
                new ShortFieldValueModel(1, null, Engine.QualityLevel.Good, DateTime.Now));
            MockedFieldValueService.GetFieldAsync("Project1", "Instance1", "Field1")
                .Returns(expectedField);

            // WHEN
            var response = await client.GetAsync("/instances/Project1/Instance1/Field1/value", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            FieldModel? retrievedField = await response.Content.ReadFromJsonAsync<FieldModel>(cancellationToken: TestContext.Current.CancellationToken);
            retrievedField.ShouldBeEquivalentTo(expectedField);
        }

        [Fact]
        public async Task WhenGettingProjectInstanceUnknownField_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedFieldValueService.GetFieldAsync("Project1", "Instance1", "UnknownField")
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.GetAsync("/instances/Project1/Instance1/UnknownField/value", TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenProject_WhenUpdatingInstanceFieldValue_ThenFieldValueIsUpdated()
        {
            // GIVEN
            FieldValueModel expectedFieldValue = new ShortFieldValueModel(5321, null, Engine.QualityLevel.Good, DateTime.Now);
            MockedFieldValueService.UpdateFieldValueAsync("Project1", "Instance1", "Field1", Arg.Any<FieldValueModel>())
                .Returns(expectedFieldValue);

            // WHEN
            var response = await client.PutAsJsonAsync("/instances/Project1/Instance1/Field1/value", expectedFieldValue, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            FieldValueModel? updatedValue = await response.Content.ReadFromJsonAsync<FieldValueModel>(cancellationToken: TestContext.Current.CancellationToken);
            updatedValue.ShouldBeEquivalentTo(expectedFieldValue);
        }

        [Fact]
        public async Task WhenUpdatingInstanceUnknownFieldValue_ThenNotFoundIsReturned()
        {
            // GIVEN
            FieldValueModel expectedFieldValue = new ShortFieldValueModel(5321, null, Engine.QualityLevel.Good, DateTime.Now);
            MockedFieldValueService.UpdateFieldValueAsync("Project1", "Instance1", "UnknownField", Arg.Any<FieldValueModel>())
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.PutAsJsonAsync("/instances/Project1/Instance1/UnknownField/value", expectedFieldValue, cancellationToken: TestContext.Current.CancellationToken);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }
    }
}
