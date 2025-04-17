namespace pva.SuperV.Model
{
    public record PagedSearchResult<T>(int PageNumber, int PageSize, int Count, List<T> Result)
    {
    }
}
