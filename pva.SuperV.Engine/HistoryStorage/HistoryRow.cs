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
        /// List of values
        /// </summary>
        public List<dynamic> Values { get; } = [];

        /// <summary>
        /// Builds a row from a TDengine row. TODO Refactor this
        /// </summary>
        /// <param name="row">TDengine row</param>
        public HistoryRow(IRows row)
        {
            Ts = ((DateTime)row.GetValue(0)).ToUniversalTime();
            for (int i = 1; i < row.FieldCount; i++)
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
