using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Classes
{
    [Description("Classes paged search request.")]
    [ExcludeFromCodeCoverage]
    public record ClassPagedSearchRequest(int PageNumber, int PageSize, string? NameFilter, string? SortOption)
        : PagedSearchRequest(PageNumber, PageSize, NameFilter, SortOption)
    {
    }
}
