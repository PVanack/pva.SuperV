using pva.Helpers.Extensions;
using pva.SuperV.Model;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.FieldDefinitions;
using Reqnroll.Assist;
using Shouldly;
using System.Net.Http.Json;

namespace pva.SuperV.TestsScenarios.StepDefinitions
{
    [Binding]
    public class ClassStepDefinitions(ScenarioContext scenarioContext) : BaseStepDefinition(scenarioContext)
    {
        [Given("Class {string} is created in project {string} with the following fields")]
        public async ValueTask CreateClassWithFields(string className, string projectId, DataTable fields)
        {
            await CreateClassWithFieldsAndBaseClass(className, null, projectId, fields);
        }

        [Given("Class {string} with base class {string} is created in project {string} with the following fields")]
        public async ValueTask CreateClassWithFieldsAndBaseClass(string className, string? baseClassName, string projectId, DataTable fields)
        {
            ClassModel expectedClass = new(className, baseClassName);
            var response = await Client.PostAsJsonAsync($"/classes/{projectId}", expectedClass);

            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            ClassModel? createdClass = await response.Content.ReadFromJsonAsync<ClassModel>();
            createdClass.ShouldBeEquivalentTo(expectedClass);

            await Createfields(projectId, className, fields);
        }

        [Then("Getting fields of class {string} of project {string} returns the following fields")]
        public async ValueTask GettingFieldsOfClasstReturnsTheFolowingFields(string className, string projectId, DataTable fields)
        {
            List<FieldDefinitionModel> expectedFieldDefinitions =
                [.. fields.Rows.Select(row => BuildFieldDefinitionModel(row))];
            var response = await Client.GetAsync($"/fields/{projectId}/{className}");

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            FieldDefinitionModel[]? fieldDefinitions = await response.Content.ReadFromJsonAsync<FieldDefinitionModel[]>();
            fieldDefinitions.ShouldBeEquivalentTo(expectedFieldDefinitions.ToArray());
        }

        [Then("Searching {string} fields of class {string} of project {string} returns the following fields")]
        public async ValueTask SearchingFieldsOfClasstReturnsTheFolowingFields(string searchedFields, string className, string projectId, DataTable fields)
        {
            List<FieldDefinitionModel> expectedFieldDefinitions =
                [.. fields.Rows.Select(row => BuildFieldDefinitionModel(row))];
            FieldDefinitionPagedSearchRequest search = new(1, 100, searchedFields, null);

            var response = await Client.PostAsJsonAsync($"/fields/{projectId}/{className}/search", search);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            var fieldDefinitions = await response.Content.ReadFromJsonAsync<PagedSearchResult<FieldDefinitionModel>>();
            fieldDefinitions!.Result.ShouldBeEquivalentTo(expectedFieldDefinitions);
        }

        private async Task Createfields(string projectId, string className, DataTable fields)
        {
            List<FieldDefinitionModel> expectedFieldDefinitions =
                [.. fields.Rows.Select(row => BuildFieldDefinitionModel(row))];
            var response = await Client.PostAsJsonAsync($"/fields/{projectId}/{className}", expectedFieldDefinitions);

            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            FieldDefinitionModel[]? createdFieldDefinitions = await response.Content.ReadFromJsonAsync<FieldDefinitionModel[]>();
            createdFieldDefinitions.ShouldBeEquivalentTo(expectedFieldDefinitions.ToArray());
        }

        private static FieldDefinitionModel BuildFieldDefinitionModel(DataTableRow row)
        {
            string fieldName = row["Name"];
            string? format = row["Format"];
            string? topicName = row["Topic"];
            if (String.IsNullOrEmpty(format))
            {
                format = null;
            }
            return row["Type"].ToLower() switch
            {
                "bool" => new BoolFieldDefinitionModel(fieldName, row.GetBoolean("Default value"), format, topicName),
                "datetime" => new DateTimeFieldDefinitionModel(fieldName, row.GetDateTime("Default value"), format, topicName),
                "double" => new DoubleFieldDefinitionModel(fieldName, row.GetDouble("Default value"), format, topicName),
                "float" => new FloatFieldDefinitionModel(fieldName, row.GetSingle("Default value"), format, topicName),
                "int" => new IntFieldDefinitionModel(fieldName, row.GetInt32("Default value"), format, topicName),
                "long" => new LongFieldDefinitionModel(fieldName, row.GetInt64("Default value"), format, topicName),
                "short" => new ShortFieldDefinitionModel(fieldName, short.CreateChecked(row.GetInt32("Default value")), format, topicName),
                "string" => new StringFieldDefinitionModel(fieldName, row["Default value"], format, topicName),
                "timespan" => new TimeSpanFieldDefinitionModel(fieldName, row["Default value"].ParseTimeSpanInvariant(), format, topicName),
                "uint" => new UintFieldDefinitionModel(fieldName, uint.CreateChecked(row.GetInt32("Default value")), format, topicName),
                "ulong" => new UlongFieldDefinitionModel(fieldName, ulong.CreateChecked(row.GetInt64("Default value")), format, topicName),
                "ushort" => new UshortFieldDefinitionModel(fieldName, ushort.CreateChecked(row.GetInt32("Default value")), format, topicName),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
