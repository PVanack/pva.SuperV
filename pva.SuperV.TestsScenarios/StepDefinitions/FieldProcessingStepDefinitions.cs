using pva.SuperV.Model.FieldProcessings;
using Shouldly;
using System.Net.Http.Json;

namespace pva.SuperV.TestsScenarios.StepDefinitions
{
    [Binding]
    public class FieldProcessingStepDefinitions(ScenarioContext scenarioContext) : BaseStepDefinition(scenarioContext)
    {
        [Given("Alarm state processing {string} is created on field {string} of class {string} of project {string}")]
        public async ValueTask AlarmStateProcessingIsCreated(string processingName, string triggeringFieldName, string className, string projectId, DataTable parameters)
        {
            DataTableRow parametersRow = parameters.Rows[0];
            FieldValueProcessingModel expectedFieldProcessing = new AlarmStateProcessingModel(processingName,
                triggeringFieldName,
                GetFieldFromParameters(parametersRow, "HighHigh limit field", false),
                GetFieldFromParameters(parametersRow, "High limit field"),
                GetFieldFromParameters(parametersRow, "Low limit field"),
                GetFieldFromParameters(parametersRow, "LowLow limit field", false),
                GetFieldFromParameters(parametersRow, "Deadband field", false),
                GetFieldFromParameters(parametersRow, "AlarmState field"),
                GetFieldFromParameters(parametersRow, "AckState field", false));
            var response = await Client.PostAsJsonAsync($"/field-processings/{projectId}/{className}/{triggeringFieldName}", expectedFieldProcessing);
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            FieldValueProcessingModel? createdFieldProcessing = await response.Content.ReadFromJsonAsync<FieldValueProcessingModel>();
            createdFieldProcessing.ShouldBeEquivalentTo(expectedFieldProcessing);
        }

        [Given("Historization processing {string} is created on field {string} of class {string} of project {string}")]
        public async ValueTask HistorizationProcessingIsCreated(string processingName, string triggeringFieldName, string className, string projectId, DataTable parameters)
        {
            DataTableRow parametersRow = parameters.Rows[0];
            List<string> fieldsToHistorize = [];
            for (int index = 1; index < parameters.Rows.Count; index++)
            {
                fieldsToHistorize.Add(parameters.Rows[index]["Field to historize"]);
            }
            FieldValueProcessingModel expectedFieldProcessing = new HistorizationProcessingModel(processingName,
                triggeringFieldName,
                GetFieldFromParameters(parametersRow, "History repository"),
                GetFieldFromParameters(parametersRow, "Timestamp field", false),
                fieldsToHistorize);
            var response = await Client.PostAsJsonAsync($"/field-processings/{projectId}/{className}/{triggeringFieldName}", expectedFieldProcessing);
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
            FieldValueProcessingModel? createdFieldProcessing = await response.Content.ReadFromJsonAsync<FieldValueProcessingModel>();
            createdFieldProcessing.ShouldBeEquivalentTo(expectedFieldProcessing);
        }

        private static string GetFieldFromParameters(DataTableRow row, string columnName)
        {
            return GetFieldFromParameters(row, columnName, true);
        }
        private static string? GetFieldFromParameters(DataTableRow row, string columnName, bool mandatory)
        {
            string fieldValue = row[columnName];
            if (String.IsNullOrEmpty(fieldValue))
            {
                return mandatory
                    ? throw new InvalidDataException(columnName)
                    : (null);
            }
            else
            {
                return (string?)fieldValue;
            }
        }
    }
}
