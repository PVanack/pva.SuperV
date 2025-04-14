namespace pva.SuperV.Model.FieldFormatters
{
    public record FieldFormatterPagedSearchRequest(int PageNumber, int PageSize, string NameFilter)
        : PagedSearchRequest(PageNumber, PageSize, NameFilter)
    {
    }
}
