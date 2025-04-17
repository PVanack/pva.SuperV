using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Field definitions paged search request.")]
    [ExcludeFromCodeCoverage]
    public record FieldDefinitionPagedSearchRequest(int PageNumber, int PageSize, string? NameFilter, string? SortOption)
        : PagedSearchRequest(PageNumber, PageSize, NameFilter, SortOption)
    {
    }
}
