namespace pva.SuperV.Model
{
    [Serializable]
    public record PagedSearchResult<T>(int PageNumber, int PageSize, int Count, List<T> Result)
    {
    }
}
