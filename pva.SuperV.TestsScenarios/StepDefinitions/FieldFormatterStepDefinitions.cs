using pva.Helpers.Extensions;
using pva.SuperV.Model.FieldFormatters;
using Shouldly;
using System.Net.Http.Json;

namespace pva.SuperV.TestsScenarios.StepDefinitions
{
    [Binding]
    public class FieldFormatterStepDefinitions(ScenarioContext scenarioContext) : BaseStepDefinition(scenarioContext)
    {
        [Given("Enum formatter {string} is created in project {string}")]
        public async ValueTask EnumFormatterIsCreated(string enumFormatterName, string projectId, DataTable enumValues)
        {
            Dictionary<int, string> values = [];
            enumValues.Rows.ForEach(row =>
            {
                int intValue = int.Parse(row["Value"]);
                values.Add(intValue, row["String"]);
            });
            EnumFormatterModel expectedFieldFormatter = new(enumFormatterName, values);
            CreateFieldFormatterRequest createRequest = new(expectedFieldFormatter);
            var response = await Client.PostAsJsonAsync($"/field-formatters/{projectId}", createRequest);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            FieldFormatterModel? fieldFormatter = await response.Content.ReadFromJsonAsync<FieldFormatterModel>();
            fieldFormatter.ShouldBeEquivalentTo(expectedFieldFormatter);
        }
    }
}
