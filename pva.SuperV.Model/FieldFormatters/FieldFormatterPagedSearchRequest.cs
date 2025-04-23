using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.FieldFormatters
{
    [Description("Field formatters paged search request.")]
    [ExcludeFromCodeCoverage]
    public record FieldFormatterPagedSearchRequest(int PageNumber, int PageSize, string? NameFilter, string? SortOption)
        : PagedSearchRequest(PageNumber, PageSize, NameFilter, SortOption)
    {
    }
}
