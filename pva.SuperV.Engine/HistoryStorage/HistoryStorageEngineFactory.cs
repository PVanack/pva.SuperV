using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pva.SuperV.Engine.HistoryStorage
{
    public static class HistoryStorageEngineFactory
    {
        public static IHistoryStorageEngine? CreateHistoryStorageEngine(string? connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                return null;
            }
            return connectionString.ToLower() switch
            {
                "fake" => new FakeHistoryStorageEngine(),
                _ => throw new ArgumentException($"Unknown history storage engine connection string: {connectionString}"),
            };
        }
    }
}
