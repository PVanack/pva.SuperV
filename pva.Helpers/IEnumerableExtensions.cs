﻿namespace pva.Helpers
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(
            this IEnumerable<T> enumerable,
            Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
        }
    }
}
