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
    private IFieldDefinitionService MockFieldDefinitionService { get => application.MockFieldDefinitionService!; }

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
        List<FieldDefinitionModel> expectedFieldDefinitions = [new IntFieldDefinitionModel("IntField")];
        MockFieldDefinitionService.GetFields("Project", "Class")
            .Returns(expectedFieldDefinitions);

        // WHEN
        var response = await client.GetAsync("/fields/Project/Class");

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        FieldDefinitionModel[]? fieldDefinitions = await response.Content.ReadFromJsonAsync<FieldDefinitionModel[]>();
        fieldDefinitions.ShouldBeEquivalentTo(expectedFieldDefinitions.ToArray());
    }

    [Fact]
    public async Task GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinition_ThenFieldDefinitionIsReturned()
    {
        // GIVEN
        IntFieldDefinitionModel expectedFieldDefinition = new IntFieldDefinitionModel("IntField");
        MockFieldDefinitionService.GetField("Project", "Class", "IntField")
            .Returns(expectedFieldDefinition);

        // WHEN
        var response = await client.GetAsync("/fields/Project/Class/IntField");

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        IntFieldDefinitionModel? fieldDefinition = await response.Content.ReadFromJsonAsync<IntFieldDefinitionModel>();
        fieldDefinition.ShouldBeEquivalentTo(expectedFieldDefinition);
    }

    [Fact]
    public async Task GivenClass_WhenCreatingClassFieldDefinition_ThenFieldDefinitionIsCreated()
    {
        // GIVEN
        FieldDefinitionModel expectedFieldDefinition = new IntFieldDefinitionModel("IntField");
        MockFieldDefinitionService.CreateField("Project", "Class", Arg.Any<FieldDefinitionModel>())
            .Returns(expectedFieldDefinition);

        // WHEN
        var response = await client.PostAsJsonAsync("/fields/Project/Class", expectedFieldDefinition);

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
        FieldDefinitionModel? fieldDefinition = await response.Content.ReadFromJsonAsync<FieldDefinitionModel>();
        fieldDefinition.ShouldBeEquivalentTo(expectedFieldDefinition);
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
