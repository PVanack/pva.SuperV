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
        private readonly Instance? instance;

        public FieldValueServiceTests()
        {
            fieldValueService = new();
            runnableProject = CreateRunnableProject();
            instance = runnableProject.CreateInstance(AllFieldsClassName, AllFieldsInstanceName);
        }

        [Fact]
        public void GivenAllFieldsInstance_WhenUpdatingFieldValues_ThenValuesAreUpdated()
        {
            DateTime timestamp = DateTime.UtcNow;
            UpdateFieldValueAndCheckResult("BoolField", new BoolFieldValueModel(true, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAndCheckResult("DateTimeField", new DateTimeFieldValueModel(timestamp, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAndCheckResult("DoubleField", new DoubleFieldValueModel(123.456, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAndCheckResult("FloatField", new FloatFieldValueModel(12.345f, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAndCheckResult("IntField", new IntFieldValueModel(123456, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAndCheckResult("LongField", new LongFieldValueModel(654321, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAndCheckResult("ShortField", new ShortFieldValueModel(1234, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAndCheckResult("StringField", new StringFieldValueModel("Hi from pva.SuperV!", QualityLevel.Good, timestamp));
            UpdateFieldValueAndCheckResult("TimeSpanField", new TimeSpanFieldValueModel(TimeSpan.FromDays(1), null, QualityLevel.Good, timestamp));
            UpdateFieldValueAndCheckResult("UintField", new UintFieldValueModel(123, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAndCheckResult("UlongField", new UlongFieldValueModel(321456, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAndCheckResult("UshortField", new UshortFieldValueModel(1456, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAndCheckResult("IntFieldWithFormat", new IntFieldValueModel(1, "High", QualityLevel.Good, timestamp));
        }

        [Fact]
        public void GivenAllFieldsInstance_WhenUpdatingFieldValuesAsString_ThenValuesAreUpdated()
        {
            DateTime timestamp = DateTime.UtcNow;
            UpdateFieldValueAsStringAndCheckResult("BoolField", "true", new BoolFieldValueModel(true, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAsStringAndCheckResult("DateTimeField", timestamp.ToString("o", CultureInfo.InvariantCulture),
                new DateTimeFieldValueModel(timestamp, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAsStringAndCheckResult("DoubleField", "123.456", new DoubleFieldValueModel(123.456, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAsStringAndCheckResult("FloatField", "12.345", new FloatFieldValueModel(12.345f, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAsStringAndCheckResult("IntField", "123456", new IntFieldValueModel(123456, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAsStringAndCheckResult("LongField", "654321", new LongFieldValueModel(654321, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAsStringAndCheckResult("ShortField", "1234", new ShortFieldValueModel(1234, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAsStringAndCheckResult("TimeSpanField", TimeSpan.FromDays(1).ToString(),
                new TimeSpanFieldValueModel(TimeSpan.FromDays(1), null, QualityLevel.Good, timestamp));
            UpdateFieldValueAsStringAndCheckResult("UintField", "123", new UintFieldValueModel(123, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAsStringAndCheckResult("UlongField", "321456", new UlongFieldValueModel(321456, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAsStringAndCheckResult("UshortField", "1456", new UshortFieldValueModel(1456, null, QualityLevel.Good, timestamp));
            UpdateFieldValueAsStringAndCheckResult("IntFieldWithFormat", "High", new IntFieldValueModel(1, "High", QualityLevel.Good, timestamp));

        }

        private void UpdateFieldValueAndCheckResult<TModel>(string fieldName, TModel expectedValueModel) where TModel : FieldValueModel
        {
            fieldValueService.UpdateFieldValue(runnableProject.GetId(), AllFieldsInstanceName, fieldName, expectedValueModel);
            FieldModel field = fieldValueService.GetField(runnableProject.GetId(), AllFieldsInstanceName, fieldName);
            field.FieldValue.ShouldBeEquivalentTo(expectedValueModel);
        }

        private void UpdateFieldValueAsStringAndCheckResult<TModel>(string fieldName, string fieldValue, TModel expectedValueModel) where TModel : FieldValueModel
        {
            StringFieldValueModel valueAsStringModel = new(fieldValue, QualityLevel.Good, expectedValueModel.Timestamp);
            fieldValueService.UpdateFieldValue(runnableProject.GetId(), AllFieldsInstanceName, fieldName, valueAsStringModel);
            FieldModel field = fieldValueService.GetField(runnableProject.GetId(), AllFieldsInstanceName, fieldName);
            field.FieldValue.ShouldBeEquivalentTo(expectedValueModel);
        }
    }
}
