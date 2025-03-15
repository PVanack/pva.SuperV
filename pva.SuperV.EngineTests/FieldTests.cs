using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using Shouldly;

namespace pva.SuperV.EngineTests
{
    [Collection("Project building")]
    public class FieldTests : SuperVTestsBase
    {
        [Theory]
        [InlineData("AS.0")]
        [InlineData("0AS")]
        [InlineData("AS-0")]
        public void GivenInvalidFieldName_WhenCreatingField_ThenInvalidFieldNameExceptionIsThrown(string invalidFieldName)
        {
            // WHEN/THEN
            Assert.Throws<InvalidIdentifierNameException>(() => new FieldDefinition<int>(invalidFieldName, 10));
        }

        [Fact]
        public void CheckValueIsAsExpectedForBoolField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<bool>(true);
        }

        [Fact]
        public void CheckValueIsAsExpectedForDateTimeField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<DateTime>(DateTime.Now);
        }

        [Fact]
        public void CheckValueIsAsExpectedForDoubleField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<double>(1234.4321);
        }

        [Fact]
        public void CheckValueIsAsExpectedForFloatField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<float>(1234.4321f);
        }

        [Fact]
        public void CheckValueIsAsExpectedForIntField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<int>(-1234);
        }

        [Fact]
        public void CheckValueIsAsExpectedForLongField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<long>(-1234);
        }

        [Fact]
        public void CheckValueIsAsExpectedForShortField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<short>(-1234);
        }

        [Fact]
        public void CheckValueIsAsExpectedForStringField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<string>("Some text");
        }

        [Fact]
        public void CheckValueIsAsExpectedForTimeSpanField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<TimeSpan>(TimeSpan.FromMinutes(1.0));
        }

        [Fact]
        public void CheckValueIsAsExpectedForUintField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<uint>(1234);
        }

        [Fact]
        public void CheckValueIsAsExpectedForUlongField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<ulong>(1234);
        }

        [Fact]
        public void CheckValueIsAsExpectedForUshortField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<ushort>(1234);
        }

        private static void GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<T>(T fieldValue)
        {
            WipProject wipProject = Project.CreateProject(ProjectName);
            _ = wipProject.AddClass(ClassName);
            wipProject.AddField(ClassName, new FieldDefinition<T>(ValueFieldName));
            RunnableProject runnableProject = Task.Run(async () => await Project.BuildAsync(wipProject)).Result;

            dynamic? instance = runnableProject.CreateInstance(ClassName, InstanceName);

            Field<T>? field = instance!.GetField<T>(ValueFieldName);
            field?.SetValue(fieldValue);

            runnableProject.SetInstanceValue<T>(InstanceName, ValueFieldName, fieldValue);

            string projectDefinitionFileName = ProjectStorage.SaveProjectDefinition(runnableProject);
            string projectInstancesFileName = ProjectStorage.SaveProjectInstances(runnableProject);

            RunnableProject? loadedProject = ProjectStorage.LoadProjectDefinition<RunnableProject>(projectDefinitionFileName);
            ProjectStorage.LoadProjectInstances(loadedProject!, projectInstancesFileName);

            Instance? loadedInstance = loadedProject!.GetInstance(InstanceName);

            field = loadedInstance!.GetField<T>(ValueFieldName);

            field!.Value.ShouldBe(fieldValue);
        }

        public record ValueToTest<T>(string StringValue, T ExpectedValue)
        {
        }

        public static TheoryData<ValueToTest<bool>> GetBoolValidValues()
             =>
                [
                 new ValueToTest<bool>("true", true),
                 new ValueToTest<bool>("false", false)
                ];
        [Theory]
        [MemberData(nameof(GetBoolValidValues))]
        public void ConvertValidStringToBool(ValueToTest<bool> valueToTest)
        {
            bool convertedValue = Field<bool>.ConvertToBool("field", valueToTest.StringValue);
            convertedValue.ShouldBe(valueToTest.ExpectedValue);
        }

        public static TheoryData<ValueToTest<DateTime>> GetDateTimeValidValues()
            =>
                [
                    new ValueToTest<DateTime>("2025-03-01T23:55:01+00:00", new DateTime(2025, 3, 1, 23, 55, 1, DateTimeKind.Utc))
                ];
        [Theory]
        [MemberData(nameof(GetDateTimeValidValues))]
        public void ConvertValidStringToDateTime(ValueToTest<DateTime> valueToTest)
        {
            DateTime convertedValue = Field<DateTime>.ConvertToDateTime("field", valueToTest.StringValue);
            convertedValue.ShouldBe(valueToTest.ExpectedValue);
        }

        public static TheoryData<ValueToTest<double>> GetDoubleValidValues()
            =>
                [
                    new ValueToTest<double>("-12.345", -12.345)
                ];
        [Theory]
        [MemberData(nameof(GetDoubleValidValues))]
        public void ConvertValidStringToDouble(ValueToTest<double> valueToTest)
        {
            double convertedValue = Field<double>.ConvertToDouble("field", valueToTest.StringValue);
            convertedValue.ShouldBe(valueToTest.ExpectedValue);
        }

        public static TheoryData<ValueToTest<int>> GetIntValidValues()
            =>
                [
                    new ValueToTest<int>("-12345", -12345)
                ];
        [Theory]
        [MemberData(nameof(GetIntValidValues))]
        public void ConvertValidStringToInt(ValueToTest<int> valueToTest)
        {
            int convertedValue = Field<int>.ConvertToInt("field", valueToTest.StringValue);
            convertedValue.ShouldBe(valueToTest.ExpectedValue);
        }

        public static TheoryData<ValueToTest<long>> GetLongValidValues()
            =>
                [
                    new ValueToTest<long>("-12345", -12345)
                ];
        [Theory]
        [MemberData(nameof(GetLongValidValues))]
        public void ConvertValidStringToLong(ValueToTest<long> valueToTest)
        {
            long convertedValue = Field<long>.ConvertToLong("field", valueToTest.StringValue);
            convertedValue.ShouldBe(valueToTest.ExpectedValue);
        }

        public static TheoryData<ValueToTest<short>> GetShortValidValues()
            =>
                [
                    new ValueToTest<short>("-12345", -12345)
                ];
        [Theory]
        [MemberData(nameof(GetShortValidValues))]
        public void ConvertValidStringToShort(ValueToTest<short> valueToTest)
        {
            short convertedValue = Field<short>.ConvertToShort("field", valueToTest.StringValue);
            convertedValue.ShouldBe(valueToTest.ExpectedValue);
        }

        public static TheoryData<ValueToTest<TimeSpan>> GetTimeSpanValidValues()
            =>
                [
                    new ValueToTest<TimeSpan>("23:55:02", new TimeSpan(23, 55, 2))
                ];
        [Theory]
        [MemberData(nameof(GetTimeSpanValidValues))]
        public void ConvertValidStringToTimeSpan(ValueToTest<TimeSpan> valueToTest)
        {
            TimeSpan convertedValue = Field<TimeSpan>.ConvertToTimeSpan("field", valueToTest.StringValue);
            convertedValue.ShouldBe(valueToTest.ExpectedValue);
        }

        public static TheoryData<ValueToTest<uint>> GetUintValidValues()
            =>
                [
                    new ValueToTest<uint>("12345", 12345)
                ];
        [Theory]
        [MemberData(nameof(GetUintValidValues))]
        public void ConvertValidStringToUint(ValueToTest<uint> valueToTest)
        {
            uint convertedValue = Field<uint>.ConvertToUint("field", valueToTest.StringValue);
            convertedValue.ShouldBe(valueToTest.ExpectedValue);
        }

        public static TheoryData<ValueToTest<ulong>> GetUlongValidValues()
            =>
                [
                    new ValueToTest<ulong>("12345", 12345)
                ];
        [Theory]
        [MemberData(nameof(GetUlongValidValues))]
        public void ConvertValidStringToUlong(ValueToTest<ulong> valueToTest)
        {
            ulong convertedValue = Field<ulong>.ConvertToUlong("field", valueToTest.StringValue);
            convertedValue.ShouldBe(valueToTest.ExpectedValue);
        }

        public static TheoryData<ValueToTest<ushort>> GetUshortValidValues()
            =>
                [
                    new ValueToTest<ushort>("12345", 12345)
                ];
        [Theory]
        [MemberData(nameof(GetUshortValidValues))]
        public void ConvertValidStringToUshort(ValueToTest<ushort> valueToTest)
        {
            ushort convertedValue = Field<ushort>.ConvertToUshort("field", valueToTest.StringValue);
            convertedValue.ShouldBe(valueToTest.ExpectedValue);
        }

        [Theory]
        [InlineData("Vrai")]
        public void ConvertInvalidStringToBool(string invalidStringValue)
        {
            Assert.Throws<StringConversionException>(() => Field<bool>.ConvertToBool("field", invalidStringValue));
        }

        [Theory]
        [InlineData("25/01/25 00:00:00")]
        public void ConvertInvalidStringToDateTime(string invalidStringValue)
        {
            Assert.Throws<StringConversionException>(() => Field<DateTime>.ConvertToDateTime("field", invalidStringValue));
        }

        [Theory]
        [InlineData("123,456")]
        public void ConvertInvalidStringToDouble(string invalidStringValue)
        {
            Assert.Throws<StringConversionException>(() => Field<double>.ConvertToDouble("field", invalidStringValue));
        }

        [Theory]
        [InlineData("123,456")]
        public void ConvertInvalidStringToFloat(string invalidStringValue)
        {
            Assert.Throws<StringConversionException>(() => Field<float>.ConvertToFloat("field", invalidStringValue));
        }

        [Theory]
        [InlineData("123.456")]
        public void ConvertInvalidStringToInt(string invalidStringValue)
        {
            Assert.Throws<StringConversionException>(() => Field<int>.ConvertToInt("field", invalidStringValue));
        }

        [Theory]
        [InlineData("123.456")]
        public void ConvertInvalidStringToLong(string invalidStringValue)
        {
            Assert.Throws<StringConversionException>(() => Field<long>.ConvertToLong("field", invalidStringValue));
        }

        [Theory]
        [InlineData("123.456")]
        public void ConvertInvalidStringToShort(string invalidStringValue)
        {
            Assert.Throws<StringConversionException>(() => Field<short>.ConvertToShort("field", invalidStringValue));
        }

        [Theory]
        [InlineData("123:75:80")]
        public void ConvertInvalidStringToTimeSpan(string invalidStringValue)
        {
            Assert.Throws<StringConversionException>(() => Field<TimeSpan>.ConvertToTimeSpan("field", invalidStringValue));
        }

        [Theory]
        [InlineData("123.456")]
        public void ConvertInvalidStringToUint(string invalidStringValue)
        {
            Assert.Throws<StringConversionException>(() => Field<uint>.ConvertToUint("field", invalidStringValue));
        }


        [Theory]
        [InlineData("123.456")]
        public void ConvertInvalidStringToUlong(string invalidStringValue)
        {
            Assert.Throws<StringConversionException>(() => Field<ulong>.ConvertToUlong("field", invalidStringValue));
        }

        [Theory]
        [InlineData("123.456")]
        public void ConvertInvalidStringToUshort(string invalidStringValue)
        {
            Assert.Throws<StringConversionException>(() => Field<ushort>.ConvertToUshort("field", invalidStringValue));
        }
    }
}