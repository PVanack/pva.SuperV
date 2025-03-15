using pva.SuperV.Engine.HistoryStorage;

namespace pva.SuperV.Model.HistoryRetrieval
{
    public static class HistoryRawRowMapper
    {
        public static List<HistoryRawRowModel> ToDto(List<HistoryRow> rows)
        {
            return [.. rows.Select(row
                => new HistoryRawRowModel(row.Ts.ToUniversalTime(), null, null, null, row.Quality,
                       [.. row.Values.Select(value => value as object)]
                    ))];
        }
    }
}
