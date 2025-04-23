using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace pva.SuperV.Model
{
    public record PagedSearchRequest(
        [property: Description("Page number. Must be greater than 0.")]
        [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
        int PageNumber,
        [property: Description("Page size. Must be greater than 0.")]
        [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
        int PageSize,
        [property: Description("Filter on name of entity.")]
        string? NameFilter,
        [property: Description("Sorting option")]
        string? SortOption)
    {
    }
}
