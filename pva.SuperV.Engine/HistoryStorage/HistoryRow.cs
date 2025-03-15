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
        /// List of values
        /// </summary>
        public List<dynamic> Values { get; } = [];

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
                Values.Add(row.GetValue(i));
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
            return Values[colIndex];
        }
    }
}
