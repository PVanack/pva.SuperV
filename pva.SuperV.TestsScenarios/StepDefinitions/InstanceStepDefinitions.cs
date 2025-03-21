using pva.SuperV.Model.Instances;
using Shouldly;
using System.Net.Http.Json;

namespace pva.SuperV.TestsScenarios.StepDefinitions
{
    [Binding]
    public class InstanceStepDefinitions : BaseStepDefinition
    {
        public InstanceStepDefinitions(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given("Instance {string} is created with class {string} in project {string}")]
        public async ValueTask InstanceIsCreated(string instanceName, string className, string projectId, DataTable fieldValues)
        {
            List<FieldModel> instanceFieds = [.. fieldValues.Rows.Select(CreateFieldModel)];
            InstanceModel expectedInstance = new(instanceName, className, instanceFieds);
            var response = await Client.PostAsJsonAsync($"/instances/{projectId}/", expectedInstance);

            // THEN
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
        }

        private static FieldModel CreateFieldModel(DataTableRow row)
        {
            string fieldName = row["Name"];
            string fieldType = row["Type"];
            string fieldValue = row["Value"];
            return new FieldModel(fieldName, fieldType,
                fieldType.ToLower() switch
                {
                    "bool" => new BoolFieldValueModel(bool.Parse(fieldValue), null, Engine.QualityLevel.Good, DateTime.Now),
                    "datetime" => new DateTimeFieldValueModel(DateTime.Parse(fieldValue), null, Engine.QualityLevel.Good, DateTime.Now),
                    "double" => new DoubleFieldValueModel(double.Parse(fieldValue), null, Engine.QualityLevel.Good, DateTime.Now),
                    "int" => new IntFieldValueModel(int.Parse(fieldValue), null, Engine.QualityLevel.Good, DateTime.Now),
                    "long" => new DoubleFieldValueModel(long.Parse(fieldValue), null, Engine.QualityLevel.Good, DateTime.Now),
                    "short" => new ShortFieldValueModel(short.Parse(fieldValue), null, Engine.QualityLevel.Good, DateTime.Now),
                    "string" => new StringFieldValueModel(fieldValue, Engine.QualityLevel.Good, DateTime.Now),
                    "timespan" => new TimeSpanFieldValueModel(TimeSpan.Parse(fieldValue), null, Engine.QualityLevel.Good, DateTime.Now),
                    "uint" => new UintFieldValueModel(uint.Parse(fieldValue), null, Engine.QualityLevel.Good, DateTime.Now),
                    "ulong" => new UlongFieldValueModel(ulong.Parse(fieldValue), null, Engine.QualityLevel.Good, DateTime.Now),
                    "ushort" => new UshortFieldValueModel(ushort.Parse(fieldValue), null, Engine.QualityLevel.Good, DateTime.Now),
                    _ => throw new NotImplementedException(),
                }
            );
        }
    }
}