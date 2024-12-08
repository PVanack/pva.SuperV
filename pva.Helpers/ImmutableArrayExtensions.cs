using System.Collections.Immutable;

namespace pva.Helpers
{
    public static class ImmutableArrayExtensions
    {
        public static void ForEach<T>(
            this ImmutableArray<T> immutableArray,
            Action<T> action)
        {
            foreach (T item in immutableArray)
            {
                action(item);
            }
        }
    }
}
