using pva.Helpers.Extensions;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.FieldDefinitions;
using Shouldly;
using System.Net.Http.Json;

namespace pva.SuperV.TestsScenarios.StepDefinitions
{
    [Binding]
    public class ClassStepDefinitions : BaseStepDefinition
    {
        public ClassStepDefinitions(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

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
            List<FieldDefinitionModel> expectedFieldDefinitions = [];
            fields.Rows.ForEach(row =>
            {
                string fieldName = row["Name"];
                string defaultValue = row["Default value"];
                string? format = row["Format"];
                if (String.IsNullOrEmpty(format))
                {
                    format = null;
                }
                expectedFieldDefinitions.Add(
                    row["Type"].ToLower() switch
                    {
                        "bool" => new BoolFieldDefinitionModel(fieldName, bool.Parse(defaultValue), format),
                        "datetime" => new DateTimeFieldDefinitionModel(fieldName, DateTime.Parse(defaultValue), format),
                        "double" => new DoubleFieldDefinitionModel(fieldName, double.Parse(defaultValue), format),
                        "int" => new IntFieldDefinitionModel(fieldName, int.Parse(defaultValue), format),
                        "long" => new DoubleFieldDefinitionModel(fieldName, long.Parse(defaultValue), format),
                        "short" => new ShortFieldDefinitionModel(fieldName, short.Parse(defaultValue), format),
                        "string" => new StringFieldDefinitionModel(fieldName, defaultValue, format),
                        "timespan" => new TimeSpanFieldDefinitionModel(fieldName, TimeSpan.Parse(defaultValue), format),
                        "uint" => new UintFieldDefinitionModel(fieldName, uint.Parse(defaultValue), format),
                        "ulong" => new UlongFieldDefinitionModel(fieldName, ulong.Parse(defaultValue), format),
                        "ushort" => new UshortFieldDefinitionModel(fieldName, ushort.Parse(defaultValue), format),
                        _ => throw new NotImplementedException(),
                    });
            });

            // WHEN
            var response = await Client.PostAsJsonAsync($"/fields/{projectId}/{className}", expectedFieldDefinitions);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            FieldDefinitionModel[]? createdFieldDefinitions = await response.Content.ReadFromJsonAsync<FieldDefinitionModel[]>();
            createdFieldDefinitions.ShouldBeEquivalentTo(expectedFieldDefinitions.ToArray());
        }
    }
}
