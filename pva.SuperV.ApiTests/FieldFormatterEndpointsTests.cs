using NSubstitute;
using NSubstitute.ExceptionExtensions;
using pva.SuperV.Api;
using pva.SuperV.Api.Services.FieldFormatters;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldFormatters;
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
            MockedFieldFormatterService.GetFieldFormatterTypes()
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
            MockedFieldFormatterService.GetFieldFormatters("Project")
                .Returns(expectedFieldFormatters);

            // WHEN
            var response = await client.GetAsync("/field-formatters/Project");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            FieldFormatterModel[]? fieldFormatters = await response.Content.ReadFromJsonAsync<FieldFormatterModel[]>();
            fieldFormatters.ShouldBeEquivalentTo(expectedFieldFormatters.ToArray());
        }

        [Fact]
        public async Task WhenGettingUnknownProjectFieldFormatters_ThenNotFoundIsReturned()
        {
            // GIVEN
            MockedFieldFormatterService.GetFieldFormatters("UnknownProject")
                .Throws<UnknownEntityException>();

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
            MockedFieldFormatterService.GetFieldFormatter("Project", expectedFieldFormatter.Name)
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
            MockedFieldFormatterService.GetFieldFormatter("Project", "UnknownFieldFormatter")
                .Throws<UnknownEntityException>();

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
            MockedFieldFormatterService.CreateFieldFormatter("Project", Arg.Any<FieldFormatterModel>())
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
            MockedFieldFormatterService.CreateFieldFormatter("UnknownProject", Arg.Any<FieldFormatterModel>())
                .Throws<UnknownEntityException>();

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
            MockedFieldFormatterService.CreateFieldFormatter("RunnableProject", Arg.Any<FieldFormatterModel>())
                .Throws<NonWipProjectException>();

            // WHEN
            var response = await client.PostAsJsonAsync($"/field-formatters/RunnableProject", createRequest);

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
            MockedFieldFormatterService.When(fake => fake.DeleteFieldFormatter("Project", "UnknownFieldFormatter"))
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
            MockedFieldFormatterService.When(fake => fake.DeleteFieldFormatter("RunnableProject", "FieldFormatter"))
                .Do(call => { throw new NonWipProjectException(); });

            // WHEN
            var response = await client.DeleteAsync($"/field-formatters/RunnableProject/FieldFormatter");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
