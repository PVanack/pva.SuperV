using pva.SuperV.Engine.Exceptions;
using TDengine.Driver;

namespace pva.SuperV.Engine.HistoryRetrieval
{
    /// <summary>
    /// Row of history for a specific timestamp.
    /// </summary>
    public class HistoryRow
    {
        /// <summary>
        /// Timestamp of history row.
        /// </summary>
        public DateTime Ts { get; }

        /// <summary>
        /// Quality of history row.
        /// </summary>
        public QualityLevel Quality { get; }

        /// <summary>
        /// List of values as objects.
        /// </summary>
        public List<object?> Values { get; } = [];

        /// <summary>
        /// Builds a row from a TDengine row.
        /// </summary>
        /// <param name="row">TDengine row</param>
        public HistoryRow(IRows row, List<IFieldDefinition> fields, bool keepFieldType)
        {
            Ts = ((DateTime)row.GetValue(row.FieldCount - 2)).ToUniversalTime();
            string qualityValueString = (string)row.GetValue(row.FieldCount - 1);
            Quality = qualityValueString != null
                ? Enum.Parse<QualityLevel>(qualityValueString)
                : QualityLevel.Uncertain;
            for (int i = 0; i < fields.Count; i++)
            {
                if (keepFieldType)
                {
                    IFieldDefinition field = fields[i];
                    Values.Add(field switch
                    {
                        FieldDefinition<bool> => ConvertToBool(field.Name, row.GetValue(i)),
                        FieldDefinition<DateTime> => ConvertToDatetime(field.Name, row.GetValue(i)),
                        FieldDefinition<double> => ConvertToDouble(field.Name, row.GetValue(i)),
                        FieldDefinition<float> => ConvertToFloat(field.Name, row.GetValue(i)),
                        FieldDefinition<int> => ConvertToInt(field.Name, row.GetValue(i)),
                        FieldDefinition<long> => ConvertToLong(field.Name, row.GetValue(i)),
                        FieldDefinition<TimeSpan> => ConvertToTimeSpan(field.Name, row.GetValue(i)),
                        FieldDefinition<short> => ConvertToShort(field.Name, row.GetValue(i)),
                        FieldDefinition<string> => ConvertToStringt(field.Name, row.GetValue(i)),
                        FieldDefinition<uint> => ConvertToUint(field.Name, row.GetValue(i)),
                        FieldDefinition<ulong> => ConvertToUlong(field.Name, row.GetValue(i)),
                        FieldDefinition<ushort> => ConvertToUshort(field.Name, row.GetValue(i)),
                        _ => throw new UnhandledMappingException(nameof(HistoryRow), field?.Type.ToString()),

                    });
                }
                else
                {
                    Values.Add(row.GetValue(i));
                }
            }
        }

        private static ushort? ConvertToUshort(string fieldName, object fieldValue)
        {

            return fieldValue == null
                ? null
                : fieldValue switch
                {
                    ushort directValue => directValue,
                    double doubleValue => (ushort)Math.Truncate(doubleValue),
                    _ => throw new UnhandledFieldTypeException(fieldName, fieldValue.GetType())
                };
        }

        private static ulong? ConvertToUlong(string fieldName, object fieldValue)
        {
            return fieldValue == null
                ? null
                : fieldValue switch
                {
                    ulong directValue => directValue,
                    double doubleValue => (ulong)Math.Truncate(doubleValue),
                    _ => throw new UnhandledFieldTypeException(fieldName, fieldValue.GetType())
                };
        }

        private static uint? ConvertToUint(string fieldName, object fieldValue)
        {
            return fieldValue == null
                ? null
                : fieldValue switch
                {
                    uint directValue => directValue,
                    double doubleValue => (uint)Math.Truncate(doubleValue),
                    _ => throw new UnhandledFieldTypeException(fieldName, fieldValue.GetType())
                };
        }

        private static string? ConvertToStringt(string fieldName, object fieldValue)
        {
            return fieldValue == null
                ? null
                : fieldValue switch
                {
                    string directValue => directValue,
                    _ => throw new UnhandledFieldTypeException(fieldName, fieldValue.GetType())
                };
        }

        private static short? ConvertToShort(string fieldName, object fieldValue)
        {
            return fieldValue == null
                ? null
                : fieldValue switch
                {
                    short directValue => directValue,
                    double doubleValue => (short)Math.Truncate(doubleValue),
                    _ => throw new UnhandledFieldTypeException(fieldName, fieldValue.GetType())
                };
        }

        private static TimeSpan? ConvertToTimeSpan(string fieldName, object fieldValue)
        {
            return fieldValue == null
                ? null
                : fieldValue switch
                {
                    TimeSpan directValue => directValue,
                    long longValue => TimeSpan.FromTicks(longValue),
                    _ => throw new UnhandledFieldTypeException(fieldName, fieldValue.GetType())
                };
        }

        private static long? ConvertToLong(string fieldName, object fieldValue)
        {
            return fieldValue == null
                ? null
                : fieldValue switch
                {
                    long directValue => directValue,
                    double doubleValue => (long)Math.Truncate(doubleValue),
                    _ => throw new UnhandledFieldTypeException(fieldName, fieldValue.GetType())
                };
        }

        private static int? ConvertToInt(string fieldName, object fieldValue)
        {
            return fieldValue == null
                ? null
                : fieldValue switch
                {
                    int directValue => directValue,
                    double doubleValue => (int)Math.Truncate(doubleValue),
                    _ => throw new UnhandledFieldTypeException(fieldName, fieldValue.GetType())
                };
        }

        private static float? ConvertToFloat(string fieldName, object fieldValue)
        {
            return fieldValue == null
                ? null
                : fieldValue switch
                {
                    float directValue => directValue,
                    double doubleValue => (float)doubleValue,
                    _ => throw new UnhandledFieldTypeException(fieldName, fieldValue.GetType())
                };
        }

        private static double? ConvertToDouble(string fieldName, object fieldValue)
        {
            return fieldValue == null
                ? null
                : fieldValue switch
                {
                    double directValue => directValue,
                    _ => throw new UnhandledFieldTypeException(fieldName, fieldValue.GetType())
                };
        }

        private static DateTime? ConvertToDatetime(string fieldName, object fieldValue)
        {
            return fieldValue == null
                ? null
                : fieldValue switch
                {
                    DateTime directValue => directValue,
                    _ => throw new UnhandledFieldTypeException(fieldName, fieldValue.GetType())
                };
        }

        private static bool? ConvertToBool(string fieldName, object fieldValue)
        {
            return fieldValue == null
                ? null
                : fieldValue switch
                {
                    bool directValue => directValue,
                    _ => throw new UnhandledFieldTypeException(fieldName, fieldValue.GetType())
                };
        }

        /// <summary>
        /// Gets field value at a specific index as the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="colIndex">Index of value.</param>
        /// <returns></returns>
        public T? GetValue<T>(int colIndex)
        {
            return (T?)Values[colIndex];
        }
    }
}
