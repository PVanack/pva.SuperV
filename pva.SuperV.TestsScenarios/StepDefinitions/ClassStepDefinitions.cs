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
            if (String.IsNullOrEmpty(format))
            {
                format = null;
            }
            return row["Type"].ToLower() switch
            {
                "bool" => new BoolFieldDefinitionModel(fieldName, row.GetBoolean("Default value"), format),
                "datetime" => new DateTimeFieldDefinitionModel(fieldName, row.GetDateTime("Default value"), format),
                "double" => new DoubleFieldDefinitionModel(fieldName, row.GetDouble("Default value"), format),
                "float" => new FloatFieldDefinitionModel(fieldName, row.GetSingle("Default value"), format),
                "int" => new IntFieldDefinitionModel(fieldName, row.GetInt32("Default value"), format),
                "long" => new DoubleFieldDefinitionModel(fieldName, row.GetInt64("Default value"), format),
                "short" => new ShortFieldDefinitionModel(fieldName, short.CreateChecked(row.GetInt32("Default value")), format),
                "string" => new StringFieldDefinitionModel(fieldName, row["Default value"], format),
                "timespan" => new TimeSpanFieldDefinitionModel(fieldName, TimeSpan.Parse(row["Default value"]), format),
                "uint" => new UintFieldDefinitionModel(fieldName, uint.CreateChecked(row.GetInt32("Default value")), format),
                "ulong" => new UlongFieldDefinitionModel(fieldName, ulong.CreateChecked(row.GetInt64("Default value")), format),
                "ushort" => new UshortFieldDefinitionModel(fieldName, ushort.CreateChecked(row.GetInt32("Default value")), format),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
