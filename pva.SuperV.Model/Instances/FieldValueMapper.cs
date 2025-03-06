using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Model.Instances
{
    public static class FieldValueMapper
    {
        public static FieldValueModel ToDto(IField field)
        {
            return field switch
            {
                Field<bool> boolField => new BoolFieldValueModel(boolField.Value, FormatValue(boolField), boolField.Quality, boolField.Timestamp),
                Field<DateTime> dateTimeField => new DateTimeFieldValueModel(dateTimeField.Value, FormatValue(dateTimeField), dateTimeField.Quality, dateTimeField.Timestamp),
                Field<double> doubleField => new DoubleFieldValueModel(doubleField.Value, FormatValue(doubleField), doubleField.Quality, doubleField.Timestamp),
                Field<float> floatField => new FloatFieldValueModel(floatField.Value, FormatValue(floatField), floatField.Quality, floatField.Timestamp),
                Field<int> intField => new IntFieldValueModel(intField.Value, FormatValue(intField), intField.Quality, intField.Timestamp),
                Field<long> longField => new LongFieldValueModel(longField.Value, FormatValue(longField), longField.Quality, longField.Timestamp),
                Field<short> shortField => new ShortFieldValueModel(shortField.Value, FormatValue(shortField), shortField.Quality, shortField.Timestamp),
                Field<string> stringField => new StringFieldValueModel(stringField.Value, stringField.Quality, stringField.Timestamp),
                Field<TimeSpan> timeSpanField => new TimeSpanFieldValueModel(timeSpanField.Value, FormatValue(timeSpanField), timeSpanField.Quality, timeSpanField.Timestamp),
                Field<uint> uintField => new UintFieldValueModel(uintField.Value, FormatValue(uintField), uintField.Quality, uintField.Timestamp),
                Field<ulong> ulongField => new UlongFieldValueModel(ulongField.Value, FormatValue(ulongField), ulongField.Quality, ulongField.Timestamp),
                Field<ushort> ushortField => new UshortFieldValueModel(ushortField.Value, FormatValue(ushortField), ushortField.Quality, ushortField.Timestamp),
                _ => throw new UnhandledMappingException(nameof(FieldValueMapper), field.Type.ToString())
            };
        }

        private static string? FormatValue<T>(Field<T> field)
        {
            return field.FieldDefinition?.Formatter?.ConvertToString(field.Value) ?? null;
        }

        public static void SetFieldValue(IField field, FieldValueModel value)
        {
            (value switch
            {
                BoolFieldValueModel derivedField => new Action(() => Field<bool>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                DateTimeFieldValueModel derivedField => new Action(() => Field<DateTime>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                DoubleFieldValueModel derivedField => new Action(() => Field<double>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                FloatFieldValueModel derivedField => new Action(() => Field<float>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                IntFieldValueModel derivedField => new Action(() => Field<int>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                LongFieldValueModel derivedField => new Action(() => Field<long>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                ShortFieldValueModel derivedField => new Action(() => Field<short>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                StringFieldValueModel derivedField => new Action(() => Field<string>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                TimeSpanFieldValueModel derivedField => new Action(() => Field<TimeSpan>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                UintFieldValueModel derivedField => new Action(() => Field<uint>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                UlongFieldValueModel derivedField => new Action(() => Field<ulong>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                UshortFieldValueModel derivedField => new Action(() => Field<ushort>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                _ => new Action(() => throw new UnhandledFieldTypeException(field.FieldDefinition!.Name, field.Type))
            })();
        }

    }
}
