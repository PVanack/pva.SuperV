using TDengine.Driver;

namespace pva.SuperV.Engine
{
    public class HistoryRow
    {
        public DateTime Ts { get; set; }
        public List<dynamic> Values { get; set; } = [];

        public HistoryRow(IRows row)
        {
            Ts = ((DateTime)row.GetValue(0)).ToUniversalTime();
            for (int i = 1; i < row.FieldCount; i++)
            {
                Values.Add(row.GetValue(i));
            }
        }
        public T GetValue<T>(int colIndex)
        {
            return Values[colIndex];
        }
    }
}
