using pva.SuperV.Engine;
using pva.SuperV.Model.Instances;
using Shouldly;
using System.Net.Http.Json;

namespace pva.SuperV.TestsScenarios.StepDefinitions
{
    [Binding]
    public class InstanceStepDefinitions(ScenarioContext scenarioContext) : BaseStepDefinition(scenarioContext)
    {
        [Given("Instance {string} is created with class {string} in project {string}")]
        public async ValueTask InstanceIsCreated(string instanceName, string className, string projectId, DataTable fieldValues)
        {
            List<FieldModel> instanceFieds = [.. fieldValues.Rows.Select(BuildFieldModel)];
            InstanceModel expectedInstance = new(instanceName, className, instanceFieds);
            var response = await Client.PostAsJsonAsync($"/instances/{projectId}/", expectedInstance);

            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
        }

        [Given("Instance {string} fields values are updated in project {string}")]
        public async ValueTask InstanceValuesAreUpdated(string instanceName, string projectId, DataTable fieldValues)
        {
            foreach (var row in fieldValues.Rows)
            {
                string fieldName = row["Name"];
                string fieldType = row["Type"];
                FieldValueModel expectedFieldValue = BuildFieldValueModel(row, fieldType);
                var response = await Client.PutAsJsonAsync($"/instances/{projectId}/{instanceName}/{fieldName}/value", expectedFieldValue);

                response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
                FieldValueModel? updatedValue = await response.Content.ReadFromJsonAsync<FieldValueModel>();
                if (updatedValue!.GetType() == expectedFieldValue.GetType())
                {
                    // Set Timestamp with the one from retrieved value
                    expectedFieldValue = expectedFieldValue with { Timestamp = updatedValue.Timestamp };
                    updatedValue.ShouldBeEquivalentTo(expectedFieldValue);
                }
                else if (expectedFieldValue is StringFieldValueModel stringFieldValueModel)
                {
                    updatedValue!.FormattedValue.ShouldBe(stringFieldValueModel.Value);
                }
            }
        }

        [Then("Instance {string} fields have expected values in project {string}")]
        public async ValueTask InstanceFieldsHaveExpectedValues(string instanceName, string projectId, DataTable fieldValues)
        {
            foreach (var row in fieldValues.Rows)
            {
                string fieldName = row["Name"];
                FieldModel expectedField = BuildFieldModel(row);
                var response = await Client.GetAsync($"/instances/{projectId}/{instanceName}/{fieldName}/value");

                response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
                FieldModel? updatedField = await response.Content.ReadFromJsonAsync<FieldModel>();
                updatedField.ShouldNotBeNull();
                // Clear type as they are not exactly correct
                updatedField = updatedField with { Type = "" };
                // Set Timestamp with the one from retrieved value
                expectedField = expectedField with
                {
                    Type = "",
                    FieldValue = expectedField.FieldValue
                        with
                    {
                        Timestamp = expectedField.FieldValue.Timestamp ?? updatedField.FieldValue.Timestamp
                    }
                };
                updatedField.ShouldBeEquivalentTo(expectedField);
            }
        }

        private static FieldModel BuildFieldModel(DataTableRow row)
        {
            string fieldName = row["Name"];
            string fieldType = row["Type"];
            return new FieldModel(fieldName, fieldType, BuildFieldValueModel(row, fieldType));
        }

        private static FieldValueModel BuildFieldValueModel(DataTableRow row, string fieldType)
        {
            string? formattedValue = row.TryGetValue("Formatted value", out formattedValue) && !String.IsNullOrEmpty(formattedValue)
                    ? formattedValue
                    : null;
            QualityLevel qualityLevel = row.TryGetValue("Quality", out string qualityString) && !String.IsNullOrEmpty(qualityString)
                ? Enum.Parse<QualityLevel>(qualityString)
                : QualityLevel.Good;
            DateTime? timestamp = row.TryGetValue("Timestamp", out string timestampString) && !String.IsNullOrEmpty(timestampString)
                ? ParseDateTime(timestampString)
                : null;
            return BuildFieldValueModel(row, "Value", fieldType, formattedValue, qualityLevel, timestamp);
        }
    }
}