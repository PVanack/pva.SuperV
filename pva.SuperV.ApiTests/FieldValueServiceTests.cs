using pva.SuperV.Api.Services.Instances;
using pva.SuperV.Engine;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.Instances;
using Shouldly;
using System.Globalization;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class FieldValueServiceTests : SuperVTestsBase
    {
        private readonly FieldValueService fieldValueService;
        private readonly RunnableProject runnableProject;

        public FieldValueServiceTests()
        {
            fieldValueService = new(LoggerFactory);
            runnableProject = CreateRunnableProject();
            _ = runnableProject.CreateInstance(AllFieldsClassName, AllFieldsInstanceName);
        }

        [Fact]
        public async Task GivenAllFieldsInstance_WhenUpdatingFieldValues_ThenValuesAreUpdated()
        {
            DateTime timestamp = DateTime.UtcNow;
            await UpdateFieldValueAndCheckResult("BoolField", new BoolFieldValueModel(true, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAndCheckResult("DateTimeField", new DateTimeFieldValueModel(timestamp, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAndCheckResult("DoubleField", new DoubleFieldValueModel(123.456, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAndCheckResult("FloatField", new FloatFieldValueModel(12.345f, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAndCheckResult("IntField", new IntFieldValueModel(123456, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAndCheckResult("LongField", new LongFieldValueModel(654321, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAndCheckResult("ShortField", new ShortFieldValueModel(1234, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAndCheckResult("StringField", new StringFieldValueModel("Hi from pva.SuperV!", QualityLevel.Good, timestamp));
            await UpdateFieldValueAndCheckResult("TimeSpanField", new TimeSpanFieldValueModel(TimeSpan.FromDays(1), null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAndCheckResult("UintField", new UintFieldValueModel(123, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAndCheckResult("UlongField", new UlongFieldValueModel(321456, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAndCheckResult("UshortField", new UshortFieldValueModel(1456, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAndCheckResult("IntFieldWithFormat", new IntFieldValueModel(1, "High", QualityLevel.Good, timestamp));
        }

        [Fact]
        public async Task GivenAllFieldsInstance_WhenUpdatingFieldValuesAsString_ThenValuesAreUpdated()
        {
            DateTime timestamp = DateTime.UtcNow;
            await UpdateFieldValueAsStringAndCheckResult("BoolField", "true", new BoolFieldValueModel(true, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAsStringAndCheckResult("DateTimeField", timestamp.ToString("o", CultureInfo.InvariantCulture),
                new DateTimeFieldValueModel(timestamp, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAsStringAndCheckResult("DoubleField", "123.456", new DoubleFieldValueModel(123.456, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAsStringAndCheckResult("FloatField", "12.345", new FloatFieldValueModel(12.345f, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAsStringAndCheckResult("IntField", "123456", new IntFieldValueModel(123456, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAsStringAndCheckResult("LongField", "654321", new LongFieldValueModel(654321, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAsStringAndCheckResult("ShortField", "1234", new ShortFieldValueModel(1234, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAsStringAndCheckResult("TimeSpanField", TimeSpan.FromDays(1).ToString(),
                new TimeSpanFieldValueModel(TimeSpan.FromDays(1), null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAsStringAndCheckResult("UintField", "123", new UintFieldValueModel(123, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAsStringAndCheckResult("UlongField", "321456", new UlongFieldValueModel(321456, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAsStringAndCheckResult("UshortField", "1456", new UshortFieldValueModel(1456, null, QualityLevel.Good, timestamp));
            await UpdateFieldValueAsStringAndCheckResult("IntFieldWithFormat", "High", new IntFieldValueModel(1, "High", QualityLevel.Good, timestamp));

        }

        private async ValueTask UpdateFieldValueAndCheckResult<TModel>(string fieldName, TModel expectedValueModel) where TModel : FieldValueModel
        {
            await fieldValueService.UpdateFieldValueAsync(runnableProject.GetId(), AllFieldsInstanceName, fieldName, expectedValueModel);
            FieldModel field = await fieldValueService.GetFieldAsync(runnableProject.GetId(), AllFieldsInstanceName, fieldName);
            field.FieldValue.ShouldBeEquivalentTo(expectedValueModel);
        }

        private async ValueTask UpdateFieldValueAsStringAndCheckResult<TModel>(string fieldName, string fieldValue, TModel expectedValueModel) where TModel : FieldValueModel
        {
            StringFieldValueModel valueAsStringModel = new(fieldValue, QualityLevel.Good, expectedValueModel.Timestamp);
            await fieldValueService.UpdateFieldValueAsync(runnableProject.GetId(), AllFieldsInstanceName, fieldName, valueAsStringModel);
            FieldModel field = await fieldValueService.GetFieldAsync(runnableProject.GetId(), AllFieldsInstanceName, fieldName);
            field.FieldValue.ShouldBeEquivalentTo(expectedValueModel);
        }
    }
}
