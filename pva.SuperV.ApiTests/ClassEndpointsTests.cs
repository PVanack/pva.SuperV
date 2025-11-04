using NSubstitute;
using NSubstitute.ExceptionExtensions;
using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.Services;
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
        public async Task GivenExistingClassesInProject_WhenSearchingProjectClasses_ThenClassesAreReturned()
        {
            // GIVEN
            ClassPagedSearchRequest search = new(1, 10, null, null);
            List<ClassModel> expectedClasses = [new ClassModel("Class1", null)];
            MockedClassService.SearchClassesAsync("Project1", search)
                .Returns(new PagedSearchResult<ClassModel>(1, 10, expectedClasses.Count, expectedClasses));

            // WHEN
            var response = await client.PostAsJsonAsync("/classes/Project1/search", search);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            PagedSearchResult<ClassModel>? projectClasses = await response.Content.ReadFromJsonAsync<PagedSearchResult<ClassModel>>();
            projectClasses.ShouldNotBeNull();
            projectClasses.PageNumber.ShouldBe(1);
            projectClasses.PageSize.ShouldBe(10);
            projectClasses.Count.ShouldBe(expectedClasses.Count);
            projectClasses.Result.ShouldBeEquivalentTo(expectedClasses);
        }

        [Fact]
        public async Task GivenExistingClassesInProject_WhenGettingProjectClasses_ThenClassesAreReturned()
        {
            // GIVEN
            List<ClassModel> expectedClasses = [new ClassModel("Class1", null)];
            MockedClassService.GetClassesAsync("Project1")
                .Returns(expectedClasses);

            // WHEN
            var response = await client.GetAsync("/classes/Project1");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            ClassModel[]? projectClasses = await response.Content.ReadFromJsonAsync<ClassModel[]>();
            projectClasses.ShouldBeEquivalentTo(expectedClasses.ToArray());
        }

        [Fact]
        public async Task WhenGettingUnknownProjectClasses_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedClassService.GetClassesAsync("UnknownProject")
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.GetAsync("/classes/UnknownProject");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenExistingClassesInProject_WhenGettingProjectClass_ThenClassIsReturned()
        {
            // GIVEN
            ClassModel expectedClass = new("Class1", null);
            MockedClassService.GetClassAsync("Project1", expectedClass.Name)
                .Returns(expectedClass);

            // WHEN
            var response = await client.GetAsync($"/classes/Project1/{expectedClass.Name}");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            ClassModel? retrievedClass = await response.Content.ReadFromJsonAsync<ClassModel>();
            retrievedClass.ShouldBeEquivalentTo(expectedClass);
        }

        [Fact]
        public async Task WhenGettingProjectUnknownClass_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedClassService.GetClassAsync("Project1", "UnknownClass")
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.GetAsync("/classes/Project1/UnknownClass");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenWipProject_WhenCreatingProjectClass_ThenClassIsCreated()
        {
            // GIVEN
            ClassModel expectedClass = new("Class1", null);
            MockedClassService.CreateClassAsync("Project1", Arg.Any<ClassModel>())
                .Returns(expectedClass);

            // WHEN
            var response = await client.PostAsJsonAsync("/classes/Project1", expectedClass);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            ClassModel? createdClass = await response.Content.ReadFromJsonAsync<ClassModel>();
            createdClass.ShouldBeEquivalentTo(expectedClass);
        }

        [Fact]
        public async Task GivenUnknownProjectProject_WhenCreatingProjectClass_ThenNotFoundIsReturned()
        {
            // GIVEN
            ClassModel expectedClass = new("Class1", null);
            MockedClassService.CreateClassAsync("UnknownProject", Arg.Any<ClassModel>())
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.PostAsJsonAsync("/classes/UnknownProject", expectedClass);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenNonWipProject_WhenCreatingProjectClass_ThenBadRequestIsReturned()
        {
            // GIVEN
            ClassModel expectedClass = new("Class1", null);
            MockedClassService.CreateClassAsync("RunnableProject", Arg.Any<ClassModel>())
                .ThrowsAsync<NonWipProjectException>();

            // WHEN
            var response = await client.PostAsJsonAsync("/classes/RunnableProject", expectedClass);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task GivenExistingClassInWipProject_WhenUpdatingProjectClass_ThenClassIsUpdated()
        {
            // GIVEN
            ClassModel expectedClass = new("Class1", null);
            MockedClassService.UpdateClassAsync("Project1", expectedClass.Name, Arg.Any<ClassModel>())
                .Returns(expectedClass);

            // WHEN
            var response = await client.PutAsJsonAsync($"/classes/Project1/{expectedClass.Name}", expectedClass);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            ClassModel? createdClass = await response.Content.ReadFromJsonAsync<ClassModel>();
            createdClass.ShouldBeEquivalentTo(expectedClass);
        }

        [Fact]
        public async Task GivenUnknownProjectProject_WhenUpdatingProjectClass_ThenNotFoundIsReturned()
        {
            // GIVEN
            ClassModel expectedClass = new("Class1", null);
            MockedClassService.UpdateClassAsync("UnknownProject", expectedClass.Name, Arg.Any<ClassModel>())
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.PutAsJsonAsync($"/classes/UnknownProject/{expectedClass.Name}", expectedClass);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenNonWipProject_WhenUpdatingProjectClass_ThenBadRequestIsReturned()
        {
            // GIVEN
            ClassModel expectedClass = new("Class1", null);
            MockedClassService.UpdateClassAsync("RunnableProject", expectedClass.Name, Arg.Any<ClassModel>())
                .ThrowsAsync<NonWipProjectException>();

            // WHEN
            var response = await client.PutAsJsonAsync($"/classes/RunnableProject/{expectedClass.Name}", expectedClass);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenWipProject_WhenDeletingProjectClass_ThenClassIsDeleted()
        {
            // GIVEN
            ClassModel expectedClass = new("Class1", null);

            // WHEN
            var response = await client.DeleteAsync($"/classes/Project1/{expectedClass.Name}");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task WhenDeletingUnknownProjectClass_ThenNotFoundIsReturned()
        {
            // GIVEN
            ClassModel expectedClass = new("Class1", null);
            MockedClassService.When(async (fake) => await fake.DeleteClassAsync("UnknownProject", expectedClass.Name))
                .Do(_ => throw new UnknownEntityException());

            // WHEN
            var response = await client.DeleteAsync($"/classes/UnknownProject/{expectedClass.Name}");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenDeletingNonWipProjectClass_ThenNotFoundIsReturned()
        {
            // GIVEN
            ClassModel expectedClass = new("Class1", null);
            MockedClassService.When(async (fake) => await fake.DeleteClassAsync("RunnableProject", expectedClass.Name))
                .Do(_ => throw new NonWipProjectException());

            // WHEN
            var response = await client.DeleteAsync($"/classes/RunnableProject/{expectedClass.Name}");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

    }
}
