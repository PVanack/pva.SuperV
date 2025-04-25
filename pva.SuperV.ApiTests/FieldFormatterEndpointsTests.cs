using NSubstitute;
using NSubstitute.ExceptionExtensions;
using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.FieldFormatters;
using pva.SuperV.Model.Services;
using Shouldly;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace pva.SuperV.ApiTests
{
    public class FieldFormatterEndpointsTests
    {
        public class ConsoleWriter(ITestOutputHelper output) : StringWriter
        {
            public override void WriteLine(string? value) => output.WriteLine(value);
        }

        private readonly TestProjectApplication application;
        private readonly HttpClient client;
        private IFieldFormatterService MockedFieldFormatterService { get => application.MockedFieldFormatterService!; }

        public FieldFormatterEndpointsTests(ITestOutputHelper output)
        {
            application = new();
            client = application.CreateClient();
            Console.SetOut(new ConsoleWriter(output));
        }

        [Fact]
        public async Task GivenExistingFieldFormatterTypes_WhenGettingFieldFormatterTypes_ThenFieldFormatterTypesAreReturned()
        {
            // GIVEN
            List<string> expectedFieldFormatterTypes = ["FormatterType1"];
            MockedFieldFormatterService.GetFieldFormatterTypesAsync()
                .Returns(expectedFieldFormatterTypes);

            // WHEN
            var response = await client.GetAsync("/field-formatters");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            List<string>? fieldFormatterTypes = await response.Content.ReadFromJsonAsync<List<string>>();
            fieldFormatterTypes.ShouldBeEquivalentTo(expectedFieldFormatterTypes);
        }

        [Fact]
        public async Task GivenExistingFieldFormattersInProject_WhenGettingProjectFieldFormatters_ThenFieldFormattersAreReturned()
        {
            // GIVEN
            List<FieldFormatterModel> expectedFieldFormatters = [new EnumFormatterModel("FieldFormatter", new Dictionary<int, string>() { { 1, "OFF" } })];
            MockedFieldFormatterService.GetFieldFormattersAsync("Project")
                .Returns(expectedFieldFormatters);

            // WHEN
            var response = await client.GetAsync("/field-formatters/Project");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            FieldFormatterModel[]? fieldFormatters = await response.Content.ReadFromJsonAsync<FieldFormatterModel[]>();
            fieldFormatters.ShouldBeEquivalentTo(expectedFieldFormatters.ToArray());
        }

        [Fact]
        public async Task GivenExistingFieldFormattersInProject_WhenSearchingProjectFieldFormatters_ThenPageOfFieldFormattersIsReturned()
        {
            // GIVEN
            List<FieldFormatterModel> expectedFieldFormatters = [new EnumFormatterModel("FieldFormatter", new Dictionary<int, string>() { { 1, "OFF" } })];
            FieldFormatterPagedSearchRequest search = new(1, 5, null, null);
            MockedFieldFormatterService.SearchFieldFormattersAsync("Project", search)
                .Returns(new PagedSearchResult<FieldFormatterModel>(1, 5, expectedFieldFormatters.Count, expectedFieldFormatters));

            // WHEN
            var response = await client.PostAsJsonAsync("/field-formatters/Project/search", search);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            PagedSearchResult<FieldFormatterModel>? result = await response.Content.ReadFromJsonAsync<PagedSearchResult<FieldFormatterModel>?>();
            result.ShouldNotBeNull();
            result.PageNumber.ShouldBe(1);
            result.PageSize.ShouldBe(5);
            result.Count.ShouldBe(expectedFieldFormatters.Count);
            result.Result.ShouldBeEquivalentTo(expectedFieldFormatters);
        }

        [Fact]
        public async Task WhenGettingUnknownProjectFieldFormatters_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedFieldFormatterService.GetFieldFormattersAsync("UnknownProject")
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.GetAsync("/field-formatters/UnknownProject");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenExistingFieldFormattersInProject_WhenGettingProjectFieldFormatter_ThenFieldFormatterIsReturned()
        {
            // GIVEN
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel("FieldFormatter", new Dictionary<int, string>() { { 0, "OFF" }, { 1, "ON" } });
            MockedFieldFormatterService.GetFieldFormatterAsync("Project", expectedFieldFormatter.Name)
                .Returns(expectedFieldFormatter);

            // WHEN
            var response = await client.GetAsync($"/field-formatters/Project/{expectedFieldFormatter.Name}");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            FieldFormatterModel? fieldFormatter = await response.Content.ReadFromJsonAsync<FieldFormatterModel>();
            fieldFormatter.ShouldBeEquivalentTo(expectedFieldFormatter);
        }

        [Fact]
        public async Task WhenGettingProjectUnknownFieldFormatter_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedFieldFormatterService.GetFieldFormatterAsync("Project", "UnknownFieldFormatter")
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.GetAsync($"/field-formatters/Project/UnknownFieldFormatter");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenWipProject_WhenCreatingEnumFieldFormatter_ThenFieldFormatterIsCreated()
        {
            // GIVEN
            Dictionary<int, string> values = new() { { 0, "Off" }, { 1, "ON" } };
            EnumFormatterModel expectedFieldFormatter = new("FieldFormatter", values);
            CreateFieldFormatterRequest createRequest = new(expectedFieldFormatter);
            MockedFieldFormatterService.CreateFieldFormatterAsync("Project", Arg.Any<FieldFormatterModel>())
                .Returns(expectedFieldFormatter);

            // WHEN
            var response = await client.PostAsJsonAsync($"/field-formatters/Project", createRequest);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            FieldFormatterModel? fieldFormatter = await response.Content.ReadFromJsonAsync<FieldFormatterModel>();
            fieldFormatter.ShouldBeEquivalentTo(expectedFieldFormatter);
        }

        [Fact]
        public async Task WhenCreatingEnumFieldFormatterOnUnknownProject_ThenNotFoundIsReturned()
        {
            // GIVEN
            Dictionary<int, string> values = new() { { 0, "Off" }, { 1, "ON" } };
            EnumFormatterModel expectedFieldFormatter = new("FieldFormatter", values);
            CreateFieldFormatterRequest createRequest = new(expectedFieldFormatter);
            MockedFieldFormatterService.CreateFieldFormatterAsync("UnknownProject", Arg.Any<FieldFormatterModel>())
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.PostAsJsonAsync($"/field-formatters/UnknownProject", createRequest);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenCreatingEnumFieldFormatterOnNonWipProject_ThenBadRequestIsReturned()
        {
            // GIVEN
            Dictionary<int, string> values = new() { { 0, "Off" }, { 1, "ON" } };
            EnumFormatterModel expectedFieldFormatter = new("FieldFormatter", values);
            CreateFieldFormatterRequest createRequest = new(expectedFieldFormatter);
            MockedFieldFormatterService.CreateFieldFormatterAsync("RunnableProject", Arg.Any<FieldFormatterModel>())
                .ThrowsAsync<NonWipProjectException>();

            // WHEN
            var response = await client.PostAsJsonAsync($"/field-formatters/RunnableProject", createRequest);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenWipProject_WhenUpdatingEnumFieldFormatter_ThenFieldFormatterIsUpdated()
        {
            // GIVEN
            Dictionary<int, string> values = new() { { 0, "Off" }, { 1, "ON" } };
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel("FieldFormatter", values);
            MockedFieldFormatterService.UpdateFieldFormatterAsync("Project", expectedFieldFormatter.Name, Arg.Any<FieldFormatterModel>())
                .Returns(expectedFieldFormatter);

            // WHEN
            var response = await client.PutAsJsonAsync($"/field-formatters/Project/{expectedFieldFormatter.Name}", expectedFieldFormatter);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            FieldFormatterModel? fieldFormatter = await response.Content.ReadFromJsonAsync<FieldFormatterModel>();
            fieldFormatter.ShouldBeEquivalentTo(expectedFieldFormatter);
        }

        [Fact]
        public async Task WhenUpdatingEnumFieldFormatterOnUnknownProject_ThenNotFoundIsReturned()
        {
            // GIVEN
            Dictionary<int, string> values = new() { { 0, "Off" }, { 1, "ON" } };
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel("FieldFormatter", values);
            MockedFieldFormatterService.UpdateFieldFormatterAsync("UnknownProject", expectedFieldFormatter.Name, Arg.Any<FieldFormatterModel>())
                .ThrowsAsync<UnknownEntityException>();

            // WHEN
            var response = await client.PutAsJsonAsync($"/field-formatters/UnknownProject/{expectedFieldFormatter.Name}", expectedFieldFormatter);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenUpdatingEnumFieldFormatterOnNonWipProject_ThenBadRequestIsReturned()
        {
            // GIVEN
            Dictionary<int, string> values = new() { { 0, "Off" }, { 1, "ON" } };
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel("FieldFormatter", values);
            MockedFieldFormatterService.UpdateFieldFormatterAsync("RunnableProject", expectedFieldFormatter.Name, Arg.Any<FieldFormatterModel>())
                .ThrowsAsync<NonWipProjectException>();

            // WHEN
            var response = await client.PutAsJsonAsync($"/field-formatters/RunnableProject/{expectedFieldFormatter.Name}", expectedFieldFormatter);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenWipProject_WhenRemovingFieldFormatter_ThenFieldFormatterIsRemoved()
        {
            // GIVEN

            // WHEN
            var response = await client.DeleteAsync($"/field-formatters/Project/FieldFormatter");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task WhenRemovingUnknownFieldFormatter_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedFieldFormatterService.When(async (fake) => await fake.DeleteFieldFormatterAsync("Project", "UnknownFieldFormatter"))
                .Do(call => { throw new UnknownEntityException(); });

            // WHEN
            var response = await client.DeleteAsync($"/field-formatters/Project/UnknownFieldFormatter");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenRemovingFieldFormatterOnNonWipProject_ThenBadRequestIsReturned()
        {
            // GIVEN
            MockedFieldFormatterService.When(async (fake) => await fake.DeleteFieldFormatterAsync("RunnableProject", "FieldFormatter"))
                .Do(call => { throw new NonWipProjectException(); });

            // WHEN
            var response = await client.DeleteAsync($"/field-formatters/RunnableProject/FieldFormatter");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
