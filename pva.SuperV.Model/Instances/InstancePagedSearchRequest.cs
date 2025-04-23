using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Instances
{
    [Description("Instances paged search request.")]
    [ExcludeFromCodeCoverage]
    public record InstancePagedSearchRequest(int PageNumber, int PageSize, string? NameFilter, string? SortOption,
        [property: Description("Filter on class name.")]
        string? ClassName)
        : PagedSearchRequest(PageNumber, PageSize, NameFilter, SortOption)
    {
    }
}
