using pva.SuperV.Engine.Exceptions;
using System.Globalization;

namespace pva.SuperV.Engine
{
    public static class FieldValueSetter
    {
        public static void SetValue<T1>(IField field, T1 value, DateTime? timestamp, QualityLevel? quality)
        {
            if (field.Type.IsAssignableFrom(value?.GetType()))
            {
                ((Field<T1>)field).SetValue(value, timestamp, quality);
                return;
            }
            else if (value is string stringValue)
            {
                if (field.FieldDefinition?.Formatter is not null)
                {
                    field.FieldDefinition?.Formatter.ConvertFromString(field, value as string, timestamp, quality);
                }
                else
                {
                    (field switch
                    {
                        Field<bool> typedField => new Action(() => typedField.SetValue(ConvertToBool(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                        Field<DateTime> typedField => new Action(() => typedField.SetValue(ConvertToDateTime(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                        Field<double> typedField => new Action(() => typedField.SetValue(ConvertToDouble(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                        Field<float> typedField => new Action(() => typedField.SetValue(ConvertToFloat(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                        Field<int> typedField => new Action(() => typedField.SetValue(ConvertToInt(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                        Field<long> typedField => new Action(() => typedField.SetValue(ConvertToLong(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                        Field<short> typedField => new Action(() => typedField.SetValue(ConvertToShort(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                        Field<TimeSpan> typedField => new Action(() => typedField.SetValue(ConvertToTimeSpan(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                        Field<uint> typedField => new Action(() => typedField.SetValue(ConvertToUint(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                        Field<ulong> typedField => new Action(() => typedField.SetValue(ConvertToUlong(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                        Field<ushort> typedField => new Action(() => typedField.SetValue(ConvertToUshort(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                        _ => new Action(() => throw new UnhandledFieldTypeException(field.FieldDefinition!.Name, field.Type))
                    })();
                }
                return;
            }
            throw new UnhandledFieldTypeException(field.FieldDefinition!.Name, field.Type);
        }

        private static bool ConvertToBool(string fieldName, string stringValue)
            => Boolean.TryParse(stringValue, out bool result)
                   ? result
                   : throw new StringConversionException(fieldName, stringValue, typeof(bool));

        private static DateTime ConvertToDateTime(string fieldName, string stringValue)
            => DateTime.TryParse(stringValue, CultureInfo.InvariantCulture, out DateTime result)
                   ? result.ToUniversalTime()
                   : throw new StringConversionException(fieldName, stringValue, typeof(DateTime));

        private static double ConvertToDouble(string fieldName, string stringValue)
            => double.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double result)
                   ? result
                   : throw new StringConversionException(fieldName, stringValue, typeof(double));

        private static float ConvertToFloat(string fieldName, string stringValue)
            => float.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float result)
                   ? result
                   : throw new StringConversionException(fieldName, stringValue, typeof(float));

        private static int ConvertToInt(string fieldName, string stringValue)
            => int.TryParse(stringValue, out int result)
                   ? result
                   : throw new StringConversionException(fieldName, stringValue, typeof(int));

        private static long ConvertToLong(string fieldName, string stringValue)
            => long.TryParse(stringValue, out long result)
                   ? result
                   : throw new StringConversionException(fieldName, stringValue, typeof(long));

        private static short ConvertToShort(string fieldName, string stringValue)
            => short.TryParse(stringValue, out short result)
                   ? result
                   : throw new StringConversionException(fieldName, stringValue, typeof(short));

        private static TimeSpan ConvertToTimeSpan(string fieldName, string stringValue)
            => TimeSpan.TryParse(stringValue, CultureInfo.InvariantCulture, out TimeSpan result)
                   ? result
                   : throw new StringConversionException(fieldName, stringValue, typeof(TimeSpan));

        private static uint ConvertToUint(string fieldName, string stringValue)
            => uint.TryParse(stringValue, out uint result)
                   ? result
                   : throw new StringConversionException(fieldName, stringValue, typeof(uint));

        private static ulong ConvertToUlong(string fieldName, string stringValue)
            => ulong.TryParse(stringValue, out ulong result)
                   ? result
                   : throw new StringConversionException(fieldName, stringValue, typeof(ulong));

        private static ushort ConvertToUshort(string fieldName, string stringValue)
            => ushort.TryParse(stringValue, out ushort result)
                   ? result
                   : throw new StringConversionException(fieldName, stringValue, typeof(ushort));
    }
}
