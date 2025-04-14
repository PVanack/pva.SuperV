namespace pva.SuperV.Model.Instances
{
    public record InstancePagedSearchRequest(int PageNumber, int PageSize, string? NameFilter, string? SortOption, string? ClassName)
        : PagedSearchRequest(PageNumber, PageSize, NameFilter, SortOption)
    {
    }
}
