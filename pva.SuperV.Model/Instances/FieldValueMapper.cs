using pva.SuperV.Engine;

namespace pva.SuperV.Model.Instances
{
    public static class FieldValueMapper
    {
        public static FieldValueModel ToDto(IField field)
        {
            return field switch
            {
                Field<bool> boolField => new BoolFieldValueModel(boolField.Value, boolField.Quality, boolField.Timestamp),
                Field<DateTime> dateTimeField => new DateTimeFieldValueModel(dateTimeField.Value, dateTimeField.Quality, dateTimeField.Timestamp),
                Field<double> doubleField => new DoubleFieldValueModel(doubleField.Value, doubleField.Quality, doubleField.Timestamp),
                Field<float> floatField => new FloatFieldValueModel(floatField.Value, floatField.Quality, floatField.Timestamp),
                Field<int> intField => new IntFieldValueModel(intField.Value, intField.Quality, intField.Timestamp),
                Field<long> longField => new LongFieldValueModel(longField.Value, longField.Quality, longField.Timestamp),
                Field<short> shortField => new ShortFieldValueModel(shortField.Value, shortField.Quality, shortField.Timestamp),
                Field<string> stringField => new StringFieldValueModel(stringField.Value, stringField.Quality, stringField.Timestamp),
                Field<TimeSpan> timeSpanField => new TimeSpanFieldValueModel(timeSpanField.Value, timeSpanField.Quality, timeSpanField.Timestamp),
                Field<uint> uintField => new UintFieldValueModel(uintField.Value, uintField.Quality, uintField.Timestamp),
                Field<ulong> ulongField => new UlongFieldValueModel(ulongField.Value, ulongField.Quality, ulongField.Timestamp),
                Field<ushort> ushortField => new UshortFieldValueModel(ushortField.Value, ushortField.Quality, ushortField.Timestamp),
                _ => throw new UnhandledMappingException(nameof(FieldValueMapper), field.Type.ToString())
            };
        }
    }
}
