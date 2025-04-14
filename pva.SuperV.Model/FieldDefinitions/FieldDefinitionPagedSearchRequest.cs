namespace pva.SuperV.Model.FieldDefinitions
{
    public record FieldDefinitionPagedSearchRequest(int PageNumber, int PageSize, string NameFilter)
        : PagedSearchRequest(PageNumber, PageSize, NameFilter)
    {
    }
}
