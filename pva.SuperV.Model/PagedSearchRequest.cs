namespace pva.SuperV.Model
{
    public record PagedSearchRequest(int PageNumber, int PageSize, string? NameFilter, string? SortOption)
    {
    }
}
