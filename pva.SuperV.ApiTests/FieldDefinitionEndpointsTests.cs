using NSubstitute;
using NSubstitute.ExceptionExtensions;
using pva.SuperV.Api.Exceptions;
using pva.SuperV.Api.Services.FieldDefinitions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldDefinitions;
using Shouldly;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace pva.SuperV.ApiTests;

public class FieldDefinitionEndpointsTests
{
    public class ConsoleWriter(ITestOutputHelper output) : StringWriter
    {
        public override void WriteLine(string? value) => output.WriteLine(value);
    }

    private readonly TestProjectApplication application;
    private readonly HttpClient client;
    private IFieldDefinitionService MockedFieldDefinitionService { get => application.MockedFieldDefinitionService!; }

    public FieldDefinitionEndpointsTests(ITestOutputHelper output)
    {
        application = new();
        client = application.CreateClient();
        Console.SetOut(new ConsoleWriter(output));
    }

    [Fact]
    public async Task GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinitions_ThenFieldDefinitionsAreReturned()
    {
        // GIVEN
        List<FieldDefinitionModel> expectedFieldDefinitions = [new IntFieldDefinitionModel("IntField", default, null)];
        MockedFieldDefinitionService.GetFields("Project", "Class")
            .Returns(expectedFieldDefinitions);

        // WHEN
        var response = await client.GetAsync("/fields/Project/Class");

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        FieldDefinitionModel[]? fieldDefinitions = await response.Content.ReadFromJsonAsync<FieldDefinitionModel[]>();
        fieldDefinitions.ShouldBeEquivalentTo(expectedFieldDefinitions.ToArray());
    }

    [Fact]
    public async Task WhenGettingUnknownClassFieldDefinitions_ThenNotFoundIsReturned()
    {
        // GIVEN
        MockedFieldDefinitionService.GetFields("Project", "UnknownClass")
            .Throws<UnknownEntityException>();

        // WHEN
        var response = await client.GetAsync("/fields/Project/UnknownClass");

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetBoolField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(
            new BoolFieldDefinitionModel("BoolField", default, null));
    }

    [Fact]
    public async Task GetDateTimeField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(
            new DateTimeFieldDefinitionModel("DateTimeField", default, null));
    }

    [Fact]
    public async Task GetDoubleField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(
            new DoubleFieldDefinitionModel("DoubleField", default, null));
    }

    [Fact]
    public async Task GetFloatField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(
            new FloatFieldDefinitionModel("BoolField", default, null));
    }

    [Fact]
    public async Task GetIntField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(
            new IntFieldDefinitionModel("IntField", default, null));
    }

    [Fact]
    public async Task GetLongField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(
            new LongFieldDefinitionModel("LongField", default, null));
    }

    [Fact]
    public async Task GetShortField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(
            new ShortFieldDefinitionModel("ShortField", default, null));
    }

    [Fact]
    public async Task GetStringField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(
            new StringFieldDefinitionModel("StringField", "", null));
    }

    [Fact]
    public async Task GetTimeSpanField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(
            new TimeSpanFieldDefinitionModel("TimeSpanField", default, null));
    }

    [Fact]
    public async Task GetUintField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(
            new UintFieldDefinitionModel("UintField", default, null));
    }

    [Fact]
    public async Task GetUlongField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(
            new UlongFieldDefinitionModel("UlongField", default, null));
    }

    [Fact]
    public async Task GetUshortField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(
            new UshortFieldDefinitionModel("UshortField", default, null));
    }

    private async Task GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned<T>(T expectedFieldDefinition) where T : FieldDefinitionModel
    {
        // GIVEN
        MockedFieldDefinitionService.GetField("Project", "Class", expectedFieldDefinition.Name)
            .Returns(expectedFieldDefinition);

        // WHEN
        var response = await client.GetAsync($"/fields/Project/Class/{expectedFieldDefinition.Name}");

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        T? fieldDefinition = await response.Content.ReadFromJsonAsync<T>();
        fieldDefinition.ShouldBeEquivalentTo(expectedFieldDefinition);
    }

    [Fact]
    public async Task WhenGettingClassUnknownFieldDefinition_ThenNotFoundIsReturned()
    {
        // GIVEN
        MockedFieldDefinitionService.GetField("Project", "Class", "UnknownFieldDefinition")
            .Throws<UnknownEntityException>();

        // WHEN
        var response = await client.GetAsync($"/fields/Project/Class/UnknownFieldDefinition");

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GivenClass_WhenCreatingClassFieldDefinition_ThenFieldDefinitionIsCreated()
    {
        // GIVEN
        List<FieldDefinitionModel> expectedFieldDefinitions = [new IntFieldDefinitionModel("IntField", default, null)];
        MockedFieldDefinitionService.CreateFields("Project", "Class", Arg.Any<List<FieldDefinitionModel>>())
            .Returns(expectedFieldDefinitions);

        // WHEN
        var response = await client.PostAsJsonAsync("/fields/Project/Class", expectedFieldDefinitions);

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
        FieldDefinitionModel[]? createdFieldDefinitions = await response.Content.ReadFromJsonAsync<FieldDefinitionModel[]>();
        createdFieldDefinitions.ShouldBeEquivalentTo(expectedFieldDefinitions.ToArray());
    }

    [Fact]
    public async Task WhenCreatingUnknownClassFieldDefinition_ThenNotFoundIsReturned()
    {
        // GIVEN
        List<FieldDefinitionModel> expectedFieldDefinitions = [new IntFieldDefinitionModel("UnknownField", default, null)];
        MockedFieldDefinitionService.CreateFields("Project", "Class", Arg.Any<List<FieldDefinitionModel>>())
            .Throws<UnknownEntityException>();

        // WHEN
        var response = await client.PostAsJsonAsync("/fields/Project/Class", expectedFieldDefinitions);

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenCreatingFieldDefinitionOnRunnableProject_ThenBadRequestIsReturned()
    {
        // GIVEN
        List<FieldDefinitionModel> expectedFieldDefinitions = [new IntFieldDefinitionModel("IntField", default, null)];
        MockedFieldDefinitionService.CreateFields("RunnableProject", "Class", Arg.Any<List<FieldDefinitionModel>>())
            .Throws<NonWipProjectException>();

        // WHEN
        var response = await client.PostAsJsonAsync("/fields/RunnableProject/Class", expectedFieldDefinitions);

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GivenClassWithFieldDefinition_WhenDeletingClassFieldDefinition_ThenFieldDefinitionIsDeleted()
    {
        // GIVEN

        // WHEN
        var response = await client.DeleteAsync("/fields/Project/Class/IntField");

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task WhenDeletingUnknownClassFieldDefinition_ThenNotFoundIsReturned()
    {
        // GIVEN
        MockedFieldDefinitionService.When(fake => fake.DeleteField("Project", "UnknownClass", "IntField"))
            .Do(call => { throw new UnknownEntityException(); });

        // WHEN
        var response = await client.DeleteAsync("/fields/Project/UnknownClass/IntField");

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenDeletingClassFieldDefinitionOnNonWipProject_ThenBadRequestIsReturned()
    {
        // GIVEN
        MockedFieldDefinitionService.When(fake => fake.DeleteField("RunnableProject", "Class", "IntField"))
            .Do(call => { throw new NonWipProjectException(); });

        // WHEN
        var response = await client.DeleteAsync("/fields/RunnableProject/Class/IntField");

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
    }
}
