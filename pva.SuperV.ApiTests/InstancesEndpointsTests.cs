using NSubstitute;
using pva.SuperV.Api.Services.Instances;
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
            List<InstanceModel> expectedInstances = [
                new InstanceModel("Instance1", "Class1",
                [new FieldModel("Field1", typeof(int).ToString(), new ShortFieldValueModel(1, Engine.QualityLevel.Good, DateTime.Now))])];
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
        public async Task GivenExistingInstancesInProject_WhenGettingProjectInstance_ThenInstanceIsReturned()
        {
            // GIVEN
            InstanceModel expectedInstance =
                new("Instance1", "Class1",
                    [new FieldModel("Field1", typeof(int).ToString(), new ShortFieldValueModel(1, Engine.QualityLevel.Good, DateTime.Now))]);
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
        public async Task GivenWipProject_WhenCreatingProjectInstanceWithFieldValues_ThenInstanceIsCreatedWithFieldValues()
        {
            // GIVEN
            InstanceModel expectedInstance =
                new("Instance1", "Class1",
                    [new FieldModel("Field1", typeof(int).ToString(), new ShortFieldValueModel(1, Engine.QualityLevel.Good, DateTime.Now))]);
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
        public async Task GivenWipProject_WhenDeletingProjectInstance_ThenInstanceIsDeleted()
        {
            // GIVEN
            InstanceModel expectedInstance =
                new("Instance1", "Class1",
                    [new FieldModel("Field1", typeof(int).ToString(), new ShortFieldValueModel(1, Engine.QualityLevel.Good, DateTime.Now))]);

            // WHEN
            var response = await client.DeleteAsync($"/instances/Project1/{expectedInstance.Name}");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GivenWipProject_WhenUpdatingInstanceFieldValue_ThenFieldValueIsUpdated()
        {
            // GIVEN
            FieldValueModel expectedFieldValue = new ShortFieldValueModel(5321, Engine.QualityLevel.Good, DateTime.Now);
            MockedFieldValueService.UpdateFieldValue("Project1", "Instance1", "Field1", Arg.Any<FieldValueModel>())
                .Returns(expectedFieldValue);

            // WHEN
            var response = await client.PutAsJsonAsync($"/instances/Project1/Instance1/Field1", expectedFieldValue);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            FieldValueModel? updatedValue = await response.Content.ReadFromJsonAsync<FieldValueModel>();
            updatedValue.ShouldBeEquivalentTo(expectedFieldValue);
        }
    }
}
