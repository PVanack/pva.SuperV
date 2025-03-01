using NSubstitute;
using pva.SuperV.Api.Services.Classes;
using pva.SuperV.Api.Services.Instances;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.Instances;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
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
        private IInstanceService MockedInstanceService { get => application.MockInstanceService!; }

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
        public async Task GivenWipProject_WhenCreatingProjectInstance_ThenInstanceIsCreated()
        {
            // GIVEN
            InstanceModel expectedInstance =
                new("Instance1", "Class1",
                    [new FieldModel("Field1", typeof(int).ToString(), new ShortFieldValueModel(1, Engine.QualityLevel.Good, DateTime.Now))]);
            MockedInstanceService.CreateInstance("Project1", expectedInstance.ClassName, expectedInstance.Name)
                .Returns(expectedInstance);

            // WHEN
            var response = await client.PostAsync($"/instances/Project1/{expectedInstance.ClassName}/{expectedInstance.Name}", null);

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

    }
}
