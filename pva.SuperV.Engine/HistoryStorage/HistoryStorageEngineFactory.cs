using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pva.SuperV.Engine.HistoryStorage
{
    public static class HistoryStorageEngineFactory
    {
        public const string NullHistoryStorage = "NullHistoryStorage";
        public const string TdEngineHistoryStorage = "TDengine";

        public static IHistoryStorageEngine? CreateHistoryStorageEngine(string? connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                return null;
            }
            if (connectionString.StartsWith(NullHistoryStorage))
            {
                return new NullHistoryStorageEngine();
            }
            else if (connectionString.StartsWith(TdEngineHistoryStorage))
            {
                string tdEngineConnectionString = connectionString.Replace($"{TdEngineHistoryStorage}:", "").Trim();
                return new TDengineHistoryStorage(tdEngineConnectionString);
            }
            else
            {
                throw new ArgumentException($"Unknown history storage engine connection string: {connectionString}");
            }
        }
    }
}
