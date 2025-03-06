using NSubstitute;
using pva.SuperV.Api.Services.FieldDefinitions;
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
        List<FieldDefinitionModel> expectedFieldDefinitions = [new IntFieldDefinitionModel("IntField", default)];
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
    public async Task GetBoolField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(new BoolFieldDefinitionModel("BoolField", default));
    }

    [Fact]
    public async Task GetDateTimeField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(new DateTimeFieldDefinitionModel("DateTimeField", default));
    }

    [Fact]
    public async Task GetDoubleField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(new DoubleFieldDefinitionModel("DoubleField", default));
    }

    [Fact]
    public async Task GetFloatField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(new FloatFieldDefinitionModel("BoolField", default));
    }

    [Fact]
    public async Task GetIntField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(new IntFieldDefinitionModel("IntField", default));
    }

    [Fact]
    public async Task GetLongField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(new LongFieldDefinitionModel("LongField", default));
    }

    [Fact]
    public async Task GetShortField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(new ShortFieldDefinitionModel("ShortField", default));
    }

    [Fact]
    public async Task GetStringField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(new StringFieldDefinitionModel("StringField", ""));
    }

    [Fact]
    public async Task GetTimeSpanField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(new TimeSpanFieldDefinitionModel("TimeSpanField", default));
    }

    [Fact]
    public async Task GetUintField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(new UintFieldDefinitionModel("UintField", default));
    }

    [Fact]
    public async Task GetUlongField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(new UlongFieldDefinitionModel("UlongField", default));
    }

    [Fact]
    public async Task GetUshortField()
    {
        await GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned(new UshortFieldDefinitionModel("UshortField", default));
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
    public async Task GivenClass_WhenCreatingClassFieldDefinition_ThenFieldDefinitionIsCreated()
    {
        // GIVEN
        List<FieldDefinitionModel> expectedFieldDefinitions = [new IntFieldDefinitionModel("IntField", default)];
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
    public async Task GivenClassWithFieldDefinition_WhenDeletingClassFieldDefinition_ThenFieldDefinitionIsDeleted()
    {
        // GIVEN

        // WHEN
        var response = await client.DeleteAsync("/fields/Project/Class/IntField");

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
    }

}
