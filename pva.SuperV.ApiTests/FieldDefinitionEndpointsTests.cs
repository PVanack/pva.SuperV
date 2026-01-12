using NSubstitute;
using NSubstitute.ExceptionExtensions;
using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.Services;
using Shouldly;
using System.Net.Http.Json;

namespace pva.SuperV.ApiTests;

public class FieldDefinitionEndpointsTests
{
    public class ConsoleWriter(ITestOutputHelper output) : StringWriter
    {
        public override void WriteLine(string? value) => output.WriteLine(value!);
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
    public async Task GivenExistingFieldDefinitionsInClass_WhenSearchingClassFieldDefinitions_ThenFieldDefinitionsAreReturned()
    {
        // GIVEN
        List<FieldDefinitionModel> expectedFieldDefinitions = [new IntFieldDefinitionModel("IntField", default, null)];
        FieldDefinitionPagedSearchRequest search = new(1, 10, null, null);
        MockedFieldDefinitionService.SearchFieldsAsync("Project", "Class", Arg.Any<FieldDefinitionPagedSearchRequest>())
            .Returns(new Model.PagedSearchResult<FieldDefinitionModel>(1, 10, expectedFieldDefinitions.Count, expectedFieldDefinitions));

        // WHEN
        var response = await client.PostAsJsonAsync("/fields/Project/Class/search", search, TestContext.Current.CancellationToken);

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        PagedSearchResult<FieldDefinitionModel>? result = await response.Content.ReadFromJsonAsync<PagedSearchResult<FieldDefinitionModel>>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.PageNumber.ShouldBe(1);
        result.PageSize.ShouldBe(10);
        result.Count.ShouldBe(expectedFieldDefinitions.Count);
        result.Result.ShouldBeEquivalentTo(expectedFieldDefinitions);
    }

    [Fact]
    public async Task GivenExistingFieldDefinitionsInClass_WhenGettingClassFieldDefinitions_ThenFieldDefinitionsAreReturned()
    {
        // GIVEN
        List<FieldDefinitionModel> expectedFieldDefinitions = [new IntFieldDefinitionModel("IntField", default, null)];
        MockedFieldDefinitionService.GetFieldsAsync("Project", "Class")
            .Returns(expectedFieldDefinitions);

        // WHEN
        var response = await client.GetAsync("/fields/Project/Class", TestContext.Current.CancellationToken);

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        FieldDefinitionModel[]? fieldDefinitions = await response.Content.ReadFromJsonAsync<FieldDefinitionModel[]>(TestContext.Current.CancellationToken);
        fieldDefinitions.ShouldBeEquivalentTo(expectedFieldDefinitions.ToArray());
    }

    [Fact]
    public async Task WhenGettingUnknownClassFieldDefinitions_ThenNotFoundIsReturned()
    {
        // GIVEN
        MockedFieldDefinitionService.GetFieldsAsync("Project", "UnknownClass")
            .ThrowsAsync<UnknownEntityException>();

        // WHEN
        var response = await client.GetAsync("/fields/Project/UnknownClass", TestContext.Current.CancellationToken);

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
        MockedFieldDefinitionService.GetFieldAsync("Project", "Class", expectedFieldDefinition.Name)
            .Returns(expectedFieldDefinition);

        // WHEN
        var response = await client.GetAsync($"/fields/Project/Class/{expectedFieldDefinition.Name}", TestContext.Current.CancellationToken);

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        T? fieldDefinition = await response.Content.ReadFromJsonAsync<T>(TestContext.Current.CancellationToken);
        fieldDefinition.ShouldBeEquivalentTo(expectedFieldDefinition);
    }

    [Fact]
    public async Task WhenGettingClassUnknownFieldDefinition_ThenNotFoundIsReturned()
    {
        // GIVEN
        MockedFieldDefinitionService.GetFieldAsync("Project", "Class", "UnknownFieldDefinition")
            .ThrowsAsync<UnknownEntityException>();

        // WHEN
        var response = await client.GetAsync("/fields/Project/Class/UnknownFieldDefinition", TestContext.Current.CancellationToken);

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GivenClass_WhenCreatingClassFieldDefinition_ThenFieldDefinitionIsCreated()
    {
        // GIVEN
        List<FieldDefinitionModel> expectedFieldDefinitions = [new IntFieldDefinitionModel("IntField", default, null)];
        MockedFieldDefinitionService.CreateFieldsAsync("Project", "Class", Arg.Any<List<FieldDefinitionModel>>())
            .Returns(expectedFieldDefinitions);

        // WHEN
        var response = await client.PostAsJsonAsync("/fields/Project/Class", expectedFieldDefinitions, TestContext.Current.CancellationToken);

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
        FieldDefinitionModel[]? createdFieldDefinitions = await response.Content.ReadFromJsonAsync<FieldDefinitionModel[]>(TestContext.Current.CancellationToken);
        createdFieldDefinitions.ShouldBeEquivalentTo(expectedFieldDefinitions.ToArray());
    }

    [Fact]
    public async Task WhenCreatingUnknownClassFieldDefinition_ThenNotFoundIsReturned()
    {
        // GIVEN
        List<FieldDefinitionModel> expectedFieldDefinitions = [new IntFieldDefinitionModel("UnknownField", default, null)];
        MockedFieldDefinitionService.CreateFieldsAsync("Project", "Class", Arg.Any<List<FieldDefinitionModel>>())
            .ThrowsAsync<UnknownEntityException>();

        // WHEN
        var response = await client.PostAsJsonAsync("/fields/Project/Class", expectedFieldDefinitions, TestContext.Current.CancellationToken);

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenCreatingFieldDefinitionOnRunnableProject_ThenBadRequestIsReturned()
    {
        // GIVEN
        List<FieldDefinitionModel> expectedFieldDefinitions = [new IntFieldDefinitionModel("IntField", default, null)];
        MockedFieldDefinitionService.CreateFieldsAsync("RunnableProject", "Class", Arg.Any<List<FieldDefinitionModel>>())
            .ThrowsAsync<NonWipProjectException>();

        // WHEN
        var response = await client.PostAsJsonAsync("/fields/RunnableProject/Class", expectedFieldDefinitions, TestContext.Current.CancellationToken);

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GivenClassWithField_WhenUpdatingClassFieldDefinition_ThenFieldDefinitionIsUpdated()
    {
        // GIVEN
        FieldDefinitionModel expectedFieldDefinition = new IntFieldDefinitionModel("IntField", default, null);
        MockedFieldDefinitionService.UpdateFieldAsync("Project", "Class", expectedFieldDefinition.Name, Arg.Any<FieldDefinitionModel>())
            .Returns(expectedFieldDefinition);

        // WHEN
        var response = await client.PutAsJsonAsync($"/fields/Project/Class/{expectedFieldDefinition.Name}", expectedFieldDefinition, TestContext.Current.CancellationToken);

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        FieldDefinitionModel? updatedFieldDefinition = await response.Content.ReadFromJsonAsync<FieldDefinitionModel>(TestContext.Current.CancellationToken);
        updatedFieldDefinition.ShouldBeEquivalentTo(expectedFieldDefinition);
    }

    [Fact]
    public async Task WhenUpdatingUnknownClassFieldDefinition_ThenNotFoundIsReturned()
    {
        // GIVEN
        FieldDefinitionModel expectedFieldDefinition = new IntFieldDefinitionModel("UnknownField", default, null);
        MockedFieldDefinitionService.UpdateFieldAsync("Project", "Class", expectedFieldDefinition.Name, Arg.Any<FieldDefinitionModel>())
            .ThrowsAsync<UnknownEntityException>();

        // WHEN
        var response = await client.PutAsJsonAsync($"/fields/Project/Class/{expectedFieldDefinition.Name}", expectedFieldDefinition, TestContext.Current.CancellationToken);

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenUpdatingFieldDefinitionOnRunnableProject_ThenBadRequestIsReturned()
    {
        // GIVEN
        FieldDefinitionModel expectedFieldDefinition = new IntFieldDefinitionModel("IntField", default, null);
        MockedFieldDefinitionService.UpdateFieldAsync("RunnableProject", "Class", expectedFieldDefinition.Name, Arg.Any<FieldDefinitionModel>())
            .ThrowsAsync<NonWipProjectException>();

        // WHEN
        var response = await client.PutAsJsonAsync($"/fields/RunnableProject/Class/{expectedFieldDefinition.Name}", expectedFieldDefinition, TestContext.Current.CancellationToken);

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GivenClassWithFieldDefinition_WhenDeletingClassFieldDefinition_ThenFieldDefinitionIsDeleted()
    {
        // GIVEN

        // WHEN
        var response = await client.DeleteAsync("/fields/Project/Class/IntField", TestContext.Current.CancellationToken);

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task WhenDeletingUnknownClassFieldDefinition_ThenNotFoundIsReturned()
    {
        // GIVEN
        MockedFieldDefinitionService.When(async (fake) => await fake.DeleteFieldAsync("Project", "UnknownClass", "IntField"))
            .Do(_ => throw new UnknownEntityException());

        // WHEN
        var response = await client.DeleteAsync("/fields/Project/UnknownClass/IntField", TestContext.Current.CancellationToken);

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenDeletingClassFieldDefinitionOnNonWipProject_ThenBadRequestIsReturned()
    {
        // GIVEN
        MockedFieldDefinitionService.When(async (fake) => await fake.DeleteFieldAsync("RunnableProject", "Class", "IntField"))
            .Do(_ => throw new NonWipProjectException());

        // WHEN
        var response = await client.DeleteAsync("/fields/RunnableProject/Class/IntField", TestContext.Current.CancellationToken);

        // THEN
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
    }
}
