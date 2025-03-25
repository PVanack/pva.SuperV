using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Model.Instances
{
    public static class FieldValueMapper
    {
        public static FieldValueModel ToDto(IField field) => field switch
        {
            Field<bool> derivedField => new BoolFieldValueModel(derivedField.Value, FormatValue(derivedField), derivedField.Quality, derivedField.Timestamp),
            Field<DateTime> derivedField => new DateTimeFieldValueModel(derivedField.Value, FormatValue(derivedField), derivedField.Quality, derivedField.Timestamp),
            Field<double> derivedField => new DoubleFieldValueModel(derivedField.Value, FormatValue(derivedField), derivedField.Quality, derivedField.Timestamp),
            Field<float> derivedField => new FloatFieldValueModel(derivedField.Value, FormatValue(derivedField), derivedField.Quality, derivedField.Timestamp),
            Field<int> derivedField => new IntFieldValueModel(derivedField.Value, FormatValue(derivedField), derivedField.Quality, derivedField.Timestamp),
            Field<long> derivedField => new LongFieldValueModel(derivedField.Value, FormatValue(derivedField), derivedField.Quality, derivedField.Timestamp),
            Field<short> derivedField => new ShortFieldValueModel(derivedField.Value, FormatValue(derivedField), derivedField.Quality, derivedField.Timestamp),
            Field<string> derivedField => new StringFieldValueModel(derivedField.Value, derivedField.Quality, derivedField.Timestamp),
            Field<TimeSpan> derivedField => new TimeSpanFieldValueModel(derivedField.Value, FormatValue(derivedField), derivedField.Quality, derivedField.Timestamp),
            Field<uint> derivedField => new UintFieldValueModel(derivedField.Value, FormatValue(derivedField), derivedField.Quality, derivedField.Timestamp),
            Field<ulong> derivedField => new UlongFieldValueModel(derivedField.Value, FormatValue(derivedField), derivedField.Quality, derivedField.Timestamp),
            Field<ushort> derivedField => new UshortFieldValueModel(derivedField.Value, FormatValue(derivedField), derivedField.Quality, derivedField.Timestamp),
            _ => throw new UnhandledMappingException(nameof(FieldValueMapper), field.Type.ToString())
        };

        public static string? FormatValue<T>(Field<T> field)
        {
            return FormatValue(field.FieldDefinition, field.Value);
        }

        public static string? FormatValue<T>(IFieldDefinition? fieldDefinition, T? value)
        {
            return fieldDefinition?.Formatter?.ConvertToString(value) ?? null;
        }

        public static void SetFieldValue(IField field, FieldValueModel value)
        {
            (value switch
            {
                BoolFieldValueModel derivedField => new Action(() => FieldValueSetter.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                DateTimeFieldValueModel derivedField => new Action(() => FieldValueSetter.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                DoubleFieldValueModel derivedField => new Action(() => FieldValueSetter.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                FloatFieldValueModel derivedField => new Action(() => FieldValueSetter.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                IntFieldValueModel derivedField => new Action(() => FieldValueSetter.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                LongFieldValueModel derivedField => new Action(() => FieldValueSetter.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                ShortFieldValueModel derivedField => new Action(() => FieldValueSetter.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                StringFieldValueModel derivedField => new Action(() => FieldValueSetter.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                TimeSpanFieldValueModel derivedField => new Action(() => FieldValueSetter.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                UintFieldValueModel derivedField => new Action(() => FieldValueSetter.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                UlongFieldValueModel derivedField => new Action(() => FieldValueSetter.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                UshortFieldValueModel derivedField => new Action(() => FieldValueSetter.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                _ => new Action(() => throw new UnhandledMappingException(field.FieldDefinition!.Name, field.Type.ToString()))
            })();
        }
    }
}
