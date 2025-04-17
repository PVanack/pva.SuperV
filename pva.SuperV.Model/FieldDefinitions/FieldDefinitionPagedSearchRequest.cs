namespace pva.SuperV.Model.FieldDefinitions
{
    public record FieldDefinitionPagedSearchRequest(int PageNumber, int PageSize, string? NameFilter, string? SortOption)
        : PagedSearchRequest(PageNumber, PageSize, NameFilter, SortOption)
    {
    }
}
