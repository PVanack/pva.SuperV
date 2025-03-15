using pva.SuperV.Engine.Exceptions;
using TDengine.Driver;

namespace pva.SuperV.Engine.HistoryStorage
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
        public List<object> Values { get; } = [];

        /// <summary>
        /// Builds a row from a TDengine row.
        /// </summary>
        /// <param name="row">TDengine row</param>
        public HistoryRow(IRows row, List<IFieldDefinition> fields)
        {
            Ts = ((DateTime)row.GetValue(row.FieldCount - 2)).ToUniversalTime();
            Quality = Enum.Parse<QualityLevel>((string)row.GetValue(row.FieldCount - 1));
            for (int i = 0; i < fields.Count; i++)
            {
                IFieldDefinition field = fields[i];
                Values.Add(field switch
                {
                    FieldDefinition<bool> derivedField => (bool)row.GetValue(i),
                    FieldDefinition<DateTime> derivedField => (DateTime)row.GetValue(i),
                    FieldDefinition<double> derivedField => (double)row.GetValue(i),
                    FieldDefinition<float> derivedField => (float)row.GetValue(i),
                    FieldDefinition<int> derivedField => (int)row.GetValue(i),
                    FieldDefinition<long> derivedField => (long)row.GetValue(i),
                    FieldDefinition<TimeSpan> derivedField => (TimeSpan)row.GetValue(i),
                    FieldDefinition<short> derivedField => (short)row.GetValue(i),
                    FieldDefinition<string> derivedField => (string)row.GetValue(i),
                    FieldDefinition<uint> derivedField => (uint)row.GetValue(i),
                    FieldDefinition<ulong> derivedField => (ulong)row.GetValue(i),
                    FieldDefinition<ushort> derivedField => (ushort)row.GetValue(i),
                    _ => throw new UnhandledMappingException(nameof(HistoryRow), field?.Type.ToString()),

                });
            }
        }

        /// <summary>
        /// Gets field value at a specific index as the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="colIndex">Index of value.</param>
        /// <returns></returns>
        public T GetValue<T>(int colIndex)
        {
            return (T)Values[colIndex];
        }
    }
}
