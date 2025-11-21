using pva.Helpers.Extensions;
using pva.SuperV.Engine;
using pva.SuperV.Model.Instances;
using Reqnroll.Assist;

namespace pva.SuperV.TestsScenarios.StepDefinitions
{
    public abstract class BaseStepDefinition
    {
        protected ScenarioContext ScenarioContext { get; init; }
        protected HttpClient Client { get; init; }

        protected BaseStepDefinition(ScenarioContext scenarioContext)
        {
            ScenarioContext = scenarioContext;
            Client = ScenarioContext.GetWebClient();
        }

        protected static FieldValueModel BuildFieldValueModel(DataTableRow row, string fieldCellName, string fieldType, string? formattedValue, QualityLevel qualityLevel, DateTime? timestamp)
        {
            return fieldType.ToLower() switch
            {
                "bool" => new BoolFieldValueModel(row.GetBoolean(fieldCellName), formattedValue, qualityLevel, timestamp),
                "datetime" => new DateTimeFieldValueModel(row.GetDateTime(fieldCellName), formattedValue, qualityLevel, timestamp),
                "double" => new DoubleFieldValueModel(row.GetDouble(fieldCellName), formattedValue, qualityLevel, timestamp),
                "float" => new FloatFieldValueModel(row.GetSingle(fieldCellName), formattedValue, qualityLevel, timestamp),
                "int" => new IntFieldValueModel(row.GetInt32(fieldCellName), formattedValue, qualityLevel, timestamp),
                "long" => new LongFieldValueModel(row.GetInt64(fieldCellName), formattedValue, qualityLevel, timestamp),
                "short" => new ShortFieldValueModel(short.CreateChecked(row.GetInt32(fieldCellName)), formattedValue, qualityLevel, timestamp),
                "string" => new StringFieldValueModel(row[fieldCellName], qualityLevel, timestamp),
                "timespan" => new TimeSpanFieldValueModel(row[fieldCellName].ParseTimeSpanInvariant(), formattedValue, qualityLevel, timestamp),
                "uint" => new UintFieldValueModel(uint.CreateChecked(row.GetInt32(fieldCellName)), formattedValue, qualityLevel, timestamp),
                "ulong" => new UlongFieldValueModel(ulong.CreateChecked(row.GetInt64(fieldCellName)), formattedValue, qualityLevel, timestamp),
                "ushort" => new UshortFieldValueModel(ushort.CreateChecked(row.GetInt32(fieldCellName)), formattedValue, qualityLevel, timestamp),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
