using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Engine.HistoryStorage
{
    /// <summary>
    /// HIstory storage engine factory. Based on connection string, the appropriate history storage engine will be created.
    /// </summary>
    public static class HistoryStorageEngineFactory
    {
        /// <summary>
        /// Null history storage string.
        /// </summary>
        public const string NullHistoryStorage = "NullHistoryStorage";

        /// <summary>
        /// TDengine history storage string.
        /// </summary>
        public const string TdEngineHistoryStorage = "TDengine";

        /// <summary>
        /// Creates the appropriate history storage engine based on connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>Created <see cref="IHistoryStorageEngine"/></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IHistoryStorageEngine? CreateHistoryStorageEngine(string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return null;
            }

            if (connectionString.StartsWith(NullHistoryStorage))
            {
                return new NullHistoryStorageEngine();
            }

            if (connectionString.StartsWith(TdEngineHistoryStorage))
            {
                string tdEngineConnectionString = connectionString.Replace($"{TdEngineHistoryStorage}:", "").Trim();
                return new TDengineHistoryStorage(tdEngineConnectionString);
            }

            throw new UnknownHistoryStorageEngineException(connectionString);
        }
    }
}