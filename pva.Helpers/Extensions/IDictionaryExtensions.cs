namespace pva.Helpers.Extensions
{
    /// <summary>Extension methods on dictionnaries.</summary>
    public static class IDictionaryExtensions
    {
        /// <summary>Allows to use foreach at the end of a LINQ expression on a dictionnary</summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="iDictionary">The dictionary to be iterated on.</param>
        /// <param name="action">The action to be invoked for each item in dictionnary. It receives the key and the value as args.</param>
        public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> iDictionary, Action<TKey, TValue> action)
        {
            foreach (KeyValuePair<TKey, TValue> pair in iDictionary)
            {
                action(pair.Key, pair.Value);
            }
        }
    }
}