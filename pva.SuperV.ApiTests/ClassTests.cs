using NSubstitute;
using NSubstitute.ExceptionExtensions;
using pva.SuperV.Api;
using pva.SuperV.Model;
using Shouldly;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace pva.SuperV.ApiTests
{
    public class ClassTests
    {
        public class ConsoleWriter(ITestOutputHelper output) : StringWriter
        {
            public override void WriteLine(string? value) => output.WriteLine(value);
        }

        private readonly TestProjectApplication application;
        private readonly HttpClient client;
        private IClassService MockedClassService { get => application.MockedClassService!; }

        public ClassTests(ITestOutputHelper output)
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
            MockedClassService.GetClasses("a-1")
                .Returns(expectedClasses);

            // WHEN
            var response = await client.GetAsync("/projects/a-1/classes");

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
            MockedClassService.GetClass("a-1", expectedClass.Name)
                .Returns(expectedClass);

            // WHEN
            var response = await client.GetAsync($"/projects/a-1/classes/{expectedClass.Name}");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            ClassModel? retrievedClass = await response.Content.ReadFromJsonAsync<ClassModel>();
            retrievedClass.ShouldBeEquivalentTo(expectedClass);
        }
    }
}
