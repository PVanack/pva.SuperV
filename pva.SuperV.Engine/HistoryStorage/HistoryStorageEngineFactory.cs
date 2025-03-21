using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Engine.HistoryStorage
{
    /// <summary>
    /// HIstory storage engine factory. Based on connection string, the appropriate history storage engine will be created.
    /// </summary>
    public static class HistoryStorageEngineFactory
    {

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

            else if (connectionString.StartsWith(NullHistoryStorageEngine.Prefix))
            {
                return new NullHistoryStorageEngine();
            }

            else if (connectionString.StartsWith(TDengineHistoryStorage.Prefix))
            {
                string tdEngineConnectionString = connectionString.Replace($"{TDengineHistoryStorage.Prefix}:", "").Trim();
                return new TDengineHistoryStorage(tdEngineConnectionString);
            }

            throw new UnknownHistoryStorageEngineException(connectionString);
        }
    }
}