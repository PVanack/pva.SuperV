using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model
{
    [Description("Entities paged search result.")]
    [ExcludeFromCodeCoverage]
    [Serializable]
    public record PagedSearchResult<T>(
        [property: Description("Page number.")]
        int PageNumber,
        [property: Description("Page size.")]
        int PageSize,
        [property: Description("Total number of entities.")]
        int Count,
        [property: Description("List of entities.")]
        List<T> Result);
}
