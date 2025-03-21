using NSubstitute;
using NSubstitute.ExceptionExtensions;
using pva.SuperV.Api;
using pva.SuperV.Api.Services.Instances;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Instances;
using Shouldly;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace pva.SuperV.ApiTests
{
    public class InstancesEndpointsTests
    {
        public class ConsoleWriter(ITestOutputHelper output) : StringWriter
        {
            public override void WriteLine(string? value) => output.WriteLine(value);
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
            MockedInstanceService.GetInstances("Project1")
                .Returns(expectedInstances);

            // WHEN
            var response = await client.GetAsync("/instances/Project1");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            InstanceModel[]? projectInstances = await response.Content.ReadFromJsonAsync<InstanceModel[]>();
            projectInstances.ShouldBeEquivalentTo(expectedInstances.ToArray());
        }

        [Fact]
        public async Task WhenGettingUnknownProjectInstances_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedInstanceService.GetInstances("UnknownProject")
                .Throws<UnknownEntityException>();

            // WHEN
            var response = await client.GetAsync("/instances/UnknownProject");

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
            MockedInstanceService.GetInstance("Project1", expectedInstance.Name)
                .Returns(expectedInstance);

            // WHEN
            var response = await client.GetAsync($"/instances/Project1/{expectedInstance.Name}");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            InstanceModel? retrievedInstance = await response.Content.ReadFromJsonAsync<InstanceModel>();
            retrievedInstance.ShouldBeEquivalentTo(expectedInstance);
        }

        [Fact]
        public async Task WhenGettingProjectUnknownInstance_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedInstanceService.GetInstance("Project1", "UnknownInstance")
                .Throws<UnknownEntityException>();

            // WHEN
            var response = await client.GetAsync($"/instances/Project1/UnknownInstance");

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
            MockedInstanceService.CreateInstance("Project1", Arg.Any<InstanceModel>())
                .Returns(expectedInstance);

            // WHEN
            var response = await client.PostAsJsonAsync($"/instances/Project1/", expectedInstance);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            InstanceModel? createdInstance = await response.Content.ReadFromJsonAsync<InstanceModel>();
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
            MockedInstanceService.CreateInstance("UnknownProject", Arg.Any<InstanceModel>())
                .Throws<UnknownEntityException>();

            // WHEN
            var response = await client.PostAsJsonAsync($"/instances/UnknownProject/", expectedInstance);

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
            MockedInstanceService.CreateInstance("RunnableProject", Arg.Any<InstanceModel>())
                .Throws<NonWipProjectException>();

            // WHEN
            var response = await client.PostAsJsonAsync($"/instances/RunnableProject/", expectedInstance);

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
            var response = await client.DeleteAsync($"/instances/Project1/{expectedInstance.Name}");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task WhenDeletingInstanceOnNonWipProject_ThenBadRequestIsReturned()
        {
            // GIVEN
            MockedInstanceService.When(fake => fake.DeleteInstance("Project", "UnknownInstance"))
                .Do(call => { throw new UnknownEntityException(); });

            // WHEN
            var response = await client.DeleteAsync($"/instances/Project/UnknownInstance");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenDeletingProjectUnknownInstance_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedInstanceService.When(fake => fake.DeleteInstance("RunnableProject", "Instance"))
                .Do(call => { throw new NonWipProjectException(); });

            // WHEN
            var response = await client.DeleteAsync($"/instances/RunnableProject/Instance");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenWipProject_WhenGettingProjectInstanceField_ThenFieldIsReturned()
        {
            // GIVEN
            FieldModel expectedField = new("Field1", typeof(int).ToString(),
                new ShortFieldValueModel(1, null, Engine.QualityLevel.Good, DateTime.Now));
            MockedFieldValueService.GetField("Project1", "Instance1", "Field1")
                .Returns(expectedField);

            // WHEN
            var response = await client.GetAsync($"/instances/Project1/Instance1/Field1/value");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            FieldModel? retrievedField = await response.Content.ReadFromJsonAsync<FieldModel>();
            retrievedField.ShouldBeEquivalentTo(expectedField);
        }

        [Fact]
        public async Task WhenGettingProjectInstanceUnknownField_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedFieldValueService.GetField("Project1", "Instance1", "UnknownField")
                .Throws<UnknownEntityException>();

            // WHEN
            var response = await client.GetAsync($"/instances/Project1/Instance1/UnknownField/value");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenProject_WhenUpdatingInstanceFieldValue_ThenFieldValueIsUpdated()
        {
            // GIVEN
            FieldValueModel expectedFieldValue = new ShortFieldValueModel(5321, null, Engine.QualityLevel.Good, DateTime.Now);
            MockedFieldValueService.UpdateFieldValue("Project1", "Instance1", "Field1", Arg.Any<FieldValueModel>())
                .Returns(expectedFieldValue);

            // WHEN
            var response = await client.PutAsJsonAsync($"/instances/Project1/Instance1/Field1/value", expectedFieldValue);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            FieldValueModel? updatedValue = await response.Content.ReadFromJsonAsync<FieldValueModel>();
            updatedValue.ShouldBeEquivalentTo(expectedFieldValue);
        }

        [Fact]
        public async Task WhenUpdatingInstanceUnknownFieldValue_ThenNotFoundIsReturned()
        {
            // GIVEN
            FieldValueModel expectedFieldValue = new ShortFieldValueModel(5321, null, Engine.QualityLevel.Good, DateTime.Now);
            MockedFieldValueService.UpdateFieldValue("Project1", "Instance1", "UnknownField", Arg.Any<FieldValueModel>())
                .Throws<UnknownEntityException>();

            // WHEN
            var response = await client.PutAsJsonAsync($"/instances/Project1/Instance1/UnknownField/value", expectedFieldValue);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }
    }
}
