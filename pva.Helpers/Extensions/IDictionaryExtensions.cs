namespace pva.Helpers.Extensions
{
    public static class IDictionaryExtensions
    {
        public static void ForEach<TKey, TValue>(
            this IDictionary<TKey, TValue> iDictionary,
            Action<TKey, TValue> action)
        {
            foreach (KeyValuePair<TKey, TValue> pair in iDictionary)
            {
                action(pair.Key, pair.Value);
            }
        }
    }
}