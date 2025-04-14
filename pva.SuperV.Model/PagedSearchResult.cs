using System.Collections;

namespace pva.SuperV.Model
{
    public record PagedSearchResult<T>(int PageNumber, int PageSize, int Count, T Result) where T : ICollection
    {
    }
}
