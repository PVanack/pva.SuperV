namespace pva.SuperV.Engine.HistoryRetrieval
{
    public record HistoryStatisticTimeRange(DateTime From, DateTime To, TimeSpan Interval, FillMode? FillMode) : HistoryTimeRange(From, To)
    {
    }
}
