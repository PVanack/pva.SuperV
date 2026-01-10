using Newtonsoft.Json;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.FieldFormatters;
using Shouldly;
using Xunit.Sdk;

namespace pva.SuperV.EngineTests
{
    public class FieldValueSetterTests
    {
        [Fact]
        public void GivenUnhandledFieldType_WhenUpdatingValue_ThenUnhandledTypeExceptionIsThrown()
        {
            Field<List<int>> field = new([])
            {
                FieldDefinition = new FieldDefinition<List<int>>("Field")
            };
            Should.Throw<UnhandledFieldTypeException>(()
                => FieldValueSetter.SetValue(field, "", DateTime.Now, QualityLevel.Good));
        }
        public record ValueToTest<T> : IXunitSerializable
        {
            public string StringValue { get; set; } = String.Empty;
            public T? ExpectedValue { get; set; }
            public T? InitialValue { get; set; }
            public ValueToTest()
            {
            }

            public ValueToTest(string stringValue, T expectedValue, T initialValue)
            {
                this.StringValue = stringValue;
                this.ExpectedValue = expectedValue;
                this.InitialValue = initialValue;
            }

            public void Deserialize(IXunitSerializationInfo info)
            {
                StringValue = info.GetValue<string>(nameof(StringValue));
                ExpectedValue = JsonConvert.DeserializeObject<T>(info.GetValue<string>(nameof(ExpectedValue)));
                InitialValue = JsonConvert.DeserializeObject<T>(info.GetValue<string>(nameof(InitialValue)));
            }

            public void Serialize(IXunitSerializationInfo info)
            {
                info.AddValue(nameof(StringValue), StringValue);
                var json = JsonConvert.SerializeObject(ExpectedValue);
                info.AddValue(nameof(ExpectedValue), json);
                json = JsonConvert.SerializeObject(InitialValue);
                info.AddValue(nameof(InitialValue), json);
            }
        }

        private static EnumFormatter CreateFormatter()
            => new("Formatter", new Dictionary<int, string>() { { 0, "OFF" }, { 1, "ON" }, { -1, "FUZZY" } });

        private static Field<T> CreateField<T>(T initialValue, FieldFormatter? fieldFormatter)
            => new(initialValue)
            {
                FieldDefinition = new FieldDefinition<T>("Field")
                { Formatter = fieldFormatter }
            };

        private static void WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue<T>(ValueToTest<T> valueToTest, FieldFormatter? fieldFormatter = null)
        {
            Field<T> field = CreateField<T>(valueToTest.InitialValue!, fieldFormatter);
            FieldValueSetter.SetValue(field, valueToTest.StringValue, DateTime.Now, QualityLevel.Good);
            field.Value.ShouldBe(valueToTest.ExpectedValue);
        }

        private static void WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<T>(string invalidStringValue, FieldFormatter? fieldFormatter = null)
        {
            Field<T> field = CreateField<T>(default!, fieldFormatter);
            Should.Throw<StringConversionException>(()
                => FieldValueSetter.SetValue(field, invalidStringValue, DateTime.Now, QualityLevel.Good));
        }

        public class BoolField
        {
            public static TheoryData<ValueToTest<bool>> GetValidValues()
                 =>
                    [
                        new ValueToTest<bool>("true", true, false),
                        new ValueToTest<bool>("false", false, true)
                    ];
            [Theory]
            [MemberData(nameof(GetValidValues))]
            public void ConvertValidString(ValueToTest<bool> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest);
            }

            [Theory]
            [InlineData("Vrai")]
            public void ConvertInvalidString(string invalidStringValue)
            {
                WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<bool>(invalidStringValue);
            }

        }

        public class DateTimeField
        {
            public static TheoryData<ValueToTest<DateTime>> GetValidValues()
                =>
                    [
                        new ValueToTest<DateTime>("2025-03-01T23:55:01+00:00", new DateTime(2025, 3, 1, 23, 55, 1, DateTimeKind.Utc), DateTime.Now)
                    ];
            [Theory]
            [MemberData(nameof(GetValidValues))]
            public void ConvertValidString(ValueToTest<DateTime> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest);
            }

            [Theory]
            [InlineData("25/01/25 00:00:00")]
            public void ConvertInvalidString(string invalidStringValue)
            {
                WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<DateTime>(invalidStringValue);
            }
        }

        public class DoubleField
        {
            public static TheoryData<ValueToTest<double>> GetValidValues()
                =>
                    [
                        new ValueToTest<double>("-12.345", -12.345, 0.0)
                    ];
            [Theory]
            [MemberData(nameof(GetValidValues))]
            public void ConvertValidString(ValueToTest<double> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest);
            }

            [Theory]
            [InlineData("123,456")]
            public void ConvertInvalidString(string invalidStringValue)
            {
                WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<double>(invalidStringValue);
            }
        }

        public class FloatField
        {
            public static TheoryData<ValueToTest<float>> GetValidValues()
                =>
                    [
                        new ValueToTest<float>("-12.345", -12.345f, 0.0f)
                    ];
            [Theory]
            [MemberData(nameof(GetValidValues))]
            public void ConvertValidString(ValueToTest<float> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest);
            }

            [Theory]
            [InlineData("123,456")]
            public void ConvertInvalidString(string invalidStringValue)
            {
                WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<float>(invalidStringValue);
            }
        }

        public class IntField
        {
            public static TheoryData<ValueToTest<int>> GetValidValues()
                =>
                    [
                        new ValueToTest<int>("-12345", -12345, 0)
                    ];
            [Theory]
            [MemberData(nameof(GetValidValues))]
            public void ConvertValidString(ValueToTest<int> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest);
            }

            [Theory]
            [InlineData("123.456")]
            public void ConvertInvalidString(string invalidStringValue)
            {
                WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<int>(invalidStringValue);
            }

            public static TheoryData<ValueToTest<int>> GetFormattedValidValues()
                =>
                    [
                        new ValueToTest<int>("OFF", 0, -1),
                        new ValueToTest<int>("ON", 1, 0),
                        new ValueToTest<int>("FUZZY", -1, 0),
                    ];
            [Theory]
            [MemberData(nameof(GetFormattedValidValues))]
            public void ConvertValidFormattedString(ValueToTest<int> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest, CreateFormatter());
            }

            [Theory]
            [InlineData("UNKNOWN")]
            public void ConvertInvalidFormattedString(string invalidStringValue)
            {
                WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<int>(invalidStringValue, CreateFormatter());
            }
        }

        public class LongField
        {
            public static TheoryData<ValueToTest<long>> GetValidValues()
                =>
                    [
                        new ValueToTest<long>("-12345", -12345, 0)
                    ];
            [Theory]
            [MemberData(nameof(GetValidValues))]
            public void ConvertValidString(ValueToTest<long> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest);
            }

            [Theory]
            [InlineData("123.456")]
            public void ConvertInvalidString(string invalidStringValue)
            {
                WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<long>(invalidStringValue);
            }

            public static TheoryData<ValueToTest<long>> GetFormattedValidValues()
                =>
                    [
                        new ValueToTest<long>("OFF", 0, -1),
                        new ValueToTest<long>("ON", 1, 0),
                        new ValueToTest<long>("FUZZY", -1, 0),
                    ];
            [Theory]
            [MemberData(nameof(GetFormattedValidValues))]
            public void ConvertValidFormattedString(ValueToTest<long> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest, CreateFormatter());
            }

            [Theory]
            [InlineData("UNKNOWN")]
            public void ConvertInvalidFormattedString(string invalidStringValue)
            {
                WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<long>(invalidStringValue, CreateFormatter());
            }
        }

        public class ShortField
        {
            public static TheoryData<ValueToTest<short>> GetValidValues()
                =>
                    [
                        new ValueToTest<short>("-12345", -12345, 0)
                    ];
            [Theory]
            [MemberData(nameof(GetValidValues))]
            public void ConvertValidString(ValueToTest<short> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest);
            }

            [Theory]
            [InlineData("123.456")]
            public void ConvertInvalidString(string invalidStringValue)
            {
                WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<short>(invalidStringValue);
            }

            public static TheoryData<ValueToTest<short>> GetFormattedValidValues()
                =>
                    [
                        new ValueToTest<short>("OFF", 0, -1),
                        new ValueToTest<short>("ON", 1, 0),
                        new ValueToTest<short>("FUZZY", -1, 0),
                    ];
            [Theory]
            [MemberData(nameof(GetFormattedValidValues))]
            public void ConvertValidFormattedString(ValueToTest<short> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest, CreateFormatter());
            }

            [Theory]
            [InlineData("UNKNOWN")]
            public void ConvertInvalidFormattedString(string invalidStringValue)
            {
                WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<short>(invalidStringValue, CreateFormatter());
            }
        }

        public class StringField
        {
            public static TheoryData<ValueToTest<string>> GetValidValues()
                =>
                    [
                        new ValueToTest<string>("Hello!", "Hello!", String.Empty)
                    ];
            [Theory]
            [MemberData(nameof(GetValidValues))]
            public void ConvertValidString(ValueToTest<string> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest);
            }
        }

        public class TimeSpanField
        {
            public static TheoryData<ValueToTest<TimeSpan>> GetValidValues()
                =>
                    [
                        new ValueToTest<TimeSpan>("23:55:02", new TimeSpan(23, 55, 2), new TimeSpan(0))
                    ];
            [Theory]
            [MemberData(nameof(GetValidValues))]
            public void ConvertValidString(ValueToTest<TimeSpan> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest);
            }

            [Theory]
            [InlineData("123:75:80")]
            public void ConvertInvalidString(string invalidStringValue)
            {
                WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<TimeSpan>(invalidStringValue);
            }
        }

        public class UintField
        {
            public static TheoryData<ValueToTest<uint>> GetValidValues()
                =>
                    [
                        new ValueToTest<uint>("12345", 12345, 0)
                    ];
            [Theory]
            [MemberData(nameof(GetValidValues))]
            public void ConvertValidString(ValueToTest<uint> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest);
            }

            [Theory]
            [InlineData("123.456")]
            public void ConvertInvalidString(string invalidStringValue)
            {
                WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<uint>(invalidStringValue);
            }

            public static TheoryData<ValueToTest<uint>> GetFormattedValidValues()
                =>
                    [
                        new ValueToTest<uint>("OFF", 0, 1),
                        new ValueToTest<uint>("ON", 1, 0)
                    ];
            [Theory]
            [MemberData(nameof(GetFormattedValidValues))]
            public void ConvertValidFormattedString(ValueToTest<uint> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest, CreateFormatter());
            }

            [Theory]
            [InlineData("UNKNOWN")]
            public void ConvertInvalidFormattedString(string invalidStringValue)
            {
                WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<uint>(invalidStringValue, CreateFormatter());
            }
        }

        public class UlongField
        {
            public static TheoryData<ValueToTest<ulong>> GetValidValues()
                =>
                    [
                        new ValueToTest<ulong>("12345", 12345, 0)
                    ];
            [Theory]
            [MemberData(nameof(GetValidValues))]
            public void ConvertValidString(ValueToTest<ulong> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest);
            }

            [Theory]
            [InlineData("123.456")]
            public void ConvertInvalidString(string invalidStringValue)
            {
                WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<ulong>(invalidStringValue);
            }
            public static TheoryData<ValueToTest<ulong>> GetFormattedValidValues()
                =>
                    [
                        new ValueToTest<ulong>("OFF", 0, 1),
                        new ValueToTest<ulong>("ON", 1, 0)
                    ];
            [Theory]
            [MemberData(nameof(GetFormattedValidValues))]
            public void ConvertValidFormattedString(ValueToTest<ulong> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest, CreateFormatter());
            }

            [Theory]
            [InlineData("UNKNOWN")]
            public void ConvertInvalidFormattedString(string invalidStringValue)
            {
                WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<ulong>(invalidStringValue, CreateFormatter());
            }

        }

        public class UshortField
        {
            public static TheoryData<ValueToTest<ushort>> GetValidValues()
                =>
                    [
                        new ValueToTest<ushort>("12345", 12345, 0)
                    ];
            [Theory]
            [MemberData(nameof(GetValidValues))]
            public void ConvertValidString(ValueToTest<ushort> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest);
            }

            [Theory]
            [InlineData("123.456")]
            public void ConvertInvalidString(string invalidStringValue)
            {
                WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<ushort>(invalidStringValue);
            }
            public static TheoryData<ValueToTest<ushort>> GetFormattedValidValues()
                =>
                    [
                        new ValueToTest<ushort>("OFF", 0, 1),
                        new ValueToTest<ushort>("ON", 1, 0)
                    ];
            [Theory]
            [MemberData(nameof(GetFormattedValidValues))]
            public void ConvertValidFormattedString(ValueToTest<ushort> valueToTest)
            {
                WhenStringValueCanBeConvertedToType_ThenFieldValueIsEqualToValue(valueToTest, CreateFormatter());
            }

            [Theory]
            [InlineData("UNKNOWN")]
            public void ConvertInvalidFormattedString(string invalidStringValue)
            {
                WhenStringValueCannotBeConvertedToType_ThenExceptionIsThrown<ushort>(invalidStringValue, CreateFormatter());
            }

        }

    }
}
