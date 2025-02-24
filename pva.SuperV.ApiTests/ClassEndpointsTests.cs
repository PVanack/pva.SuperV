using NSubstitute;
using NSubstitute.Core.Arguments;
using pva.SuperV.Api;
using pva.SuperV.Model.Classes;
using Shouldly;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace pva.SuperV.ApiTests
{
    public class ClassEndpointsTests
    {
        public class ConsoleWriter(ITestOutputHelper output) : StringWriter
        {
            public override void WriteLine(string? value) => output.WriteLine(value);
        }

        private readonly TestProjectApplication application;
        private readonly HttpClient client;
        private IClassService MockedClassService { get => application.MockedClassService!; }

        public ClassEndpointsTests(ITestOutputHelper output)
        {
            application = new();
            client = application.CreateClient();
            Console.SetOut(new ConsoleWriter(output));
        }

        [Fact]
        public async Task GivenExistingClassesInProject_WhenGettingProjectClasses_ThenClassesAreReturned()
        {
            // GIVEN
            List<ClassModel> expectedClasses = [new ClassModel("Class1", null)];
            MockedClassService.GetClasses("Project1")
                .Returns(expectedClasses);

            // WHEN
            var response = await client.GetAsync("/classes/Project1");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            ClassModel[]? projectClasses = await response.Content.ReadFromJsonAsync<ClassModel[]>();
            projectClasses.ShouldBeEquivalentTo(expectedClasses.ToArray());
        }

        [Fact]
        public async Task GivenExistingClassesInProject_WhenGettingProjectClass_ThenClassIsReturned()
        {
            // GIVEN
            ClassModel expectedClass = new("Class1", null);
            MockedClassService.GetClass("Project1", expectedClass.Name)
                .Returns(expectedClass);

            // WHEN
            var response = await client.GetAsync($"/classes/Project1/{expectedClass.Name}");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            ClassModel? retrievedClass = await response.Content.ReadFromJsonAsync<ClassModel>();
            retrievedClass.ShouldBeEquivalentTo(expectedClass);
        }

        [Fact]
        public async Task GivenWipProject_WhenCreatingProjectClass_ThenClassIsCreated()
        {
            // GIVEN
            ClassModel expectedClass = new ClassModel("Class1", null);
            MockedClassService.CreateClass("Project1", Arg.Any<ClassModel>())
                .Returns(expectedClass);

            // WHEN
            var response = await client.PostAsJsonAsync("/classes/Project1/", expectedClass);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            ClassModel? createdClass = await response.Content.ReadFromJsonAsync<ClassModel>();
            createdClass.ShouldBeEquivalentTo(expectedClass);
        }

        [Fact]
        public async Task GivenWipProject_WhenDeletingProjectClass_ThenClassIsDeleted()
        {
            // GIVEN
            ClassModel expectedClass = new ClassModel("Class1", null);

            // WHEN
            var response = await client.DeleteAsync($"/classes/Project1/{expectedClass.Name}");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
        }
    }
}
