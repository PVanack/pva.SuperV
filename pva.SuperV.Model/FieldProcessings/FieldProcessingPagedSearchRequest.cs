namespace pva.SuperV.Model.FieldProcessings
{
    public record FieldProcessingPagedSearchRequest(int PageNumber, int PageSize, string NameFilter)
        : PagedSearchRequest(PageNumber, PageSize, NameFilter)
    {
    }
}
