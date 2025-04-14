namespace pva.SuperV.Model.HistoryRepositories
{
    public record HistoryRepositoryPagedSearchRequest(int PageNumber, int PageSize, string NameFilter)
        : PagedSearchRequest(PageNumber, PageSize, NameFilter)
    {
    }
}
