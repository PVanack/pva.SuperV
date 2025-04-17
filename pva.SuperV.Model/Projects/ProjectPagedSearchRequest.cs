namespace pva.SuperV.Model.Projects
{
    public record ProjectPagedSearchRequest(int PageNumber, int PageSize, string? NameFilter, string? SortOption)
        : PagedSearchRequest(PageNumber, PageSize, NameFilter, SortOption)
    {
    }
}
