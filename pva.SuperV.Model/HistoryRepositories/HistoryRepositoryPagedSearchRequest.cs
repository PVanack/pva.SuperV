namespace pva.SuperV.Model.HistoryRepositories
{
    public record HistoryRepositoryPagedSearchRequest(int PageNumber, int PageSize, string? NameFilter, string? SortOption)
        : PagedSearchRequest(PageNumber, PageSize, NameFilter, SortOption)
    {
    }
}
