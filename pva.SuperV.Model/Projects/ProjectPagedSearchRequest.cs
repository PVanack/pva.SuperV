namespace pva.SuperV.Model.Projects
{
    public record ProjectPagedSearchRequest(int PageNumber, int PageSize, string NameFilter)
        : PagedSearchRequest(PageNumber, PageSize, NameFilter)
    {
    }
}
