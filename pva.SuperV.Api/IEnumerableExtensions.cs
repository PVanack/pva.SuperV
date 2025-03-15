namespace pva.Helpers.Extensions
{
    /// <summary>Extension methods on enumerables.</summary>
    public static class IEnumerableExtensions
    {
        /// <summary>Allows to use foreach at the end of a LINQ expression on an enumerable</summary>
        /// <typeparam name="T">The type of the items contained in enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to be iterated on.</param>
        /// <param name="action">The action to be invoked for each item in enumerable. It receives the item as arg.</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
        }
    }
}