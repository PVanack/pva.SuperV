using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.Processing;
using TDengine.Driver;
using TDengine.Driver.Client;

namespace pva.SuperV.Engine.HistoryStorage
{
    public class TDengineHistoryStorage : IHistoryStorageEngine
    {
        private static readonly Dictionary<Type, string> dotnetToDbTypes = new()
            {
            { typeof(DateTime), "TIMESTAMP" },
{ typeof(short), "SMALLINT"},
            {typeof(int), "INT" },
            {typeof(long), "BIGINT" },
            {typeof(uint), "INT UNSIGNED" },
            {typeof(ulong), "BIGINT UNSIGNED" },
            {typeof(float), "FLOAT" },
            {typeof(double),  "DOUBLE" },
            {typeof(bool), "BOOL" },
            {typeof(string), "NCHAR" },
            {typeof(sbyte),  "TINYINT" },
            {typeof(byte), "TINYINT UNSIGNED" },
            {typeof(ushort), "SMALLINT UNSIGNED" }
            /*
            BINARY  byte[]
            JSON    byte[]
            VARBINARY   byte[]
            GEOMETRY    byte[]
            */
        };
        private readonly string connectionString;
        private ITDengineClient? tdEngineClient;

        public TDengineHistoryStorage(string tdEngineConnectionString)
        {
            this.connectionString = tdEngineConnectionString;
            Connect();
        }

        private void Connect()
        {
            var builder = new ConnectionStringBuilder(connectionString);
            try
            {
                // Open connection with using block, it will close the connection automatically
                tdEngineClient = DbDriver.Open(builder);
            }
            catch (Exception e)
            {
                throw new TdEngineException($"connect to {builder}", e);
            }
        }

        public string UpsertRepository(string projectName, HistoryRepository repository)
        {
            string repositoryName = $"{projectName}{repository.Name}".ToLowerInvariant();
            try
            {
                tdEngineClient?.Exec($"CREATE DATABASE IF NOT EXISTS {repositoryName} PRECISION 'ns' KEEP 3650 DURATION 10 BUFFER 16;");
            }
            catch (Exception e)
            {
                throw new TdEngineException($"upsert repository {repositoryName}", e);
            }
            return repositoryName;
        }

        public void DeleteRepository(string projectName, string repositoryName)
        {
            string repositoryActualName = $"{projectName}{repositoryName}".ToLowerInvariant();
            try
            {
                tdEngineClient?.Exec($"DROP DATABASE {repositoryActualName};");
            }
            catch (Exception e)
            {
                throw new TdEngineException($"delete repository {repositoryActualName}", e);
            }
        }

        public string UpsertClassTimeSerie<T>(string repositoryStorageId, string projectName, string className, HistorizationProcessing<T> historizationProcessing)
        {
            string tableName = $"{projectName}{className}{historizationProcessing.Name}".ToLowerInvariant();
            try
            {
                tdEngineClient?.Exec($"USE {repositoryStorageId};");
                string fieldNames = "TS TIMESTAMP,";
                fieldNames +=
                    historizationProcessing.FieldsToHistorize
                        .Select(field => $"_{field.Name} {GetFieldDbType(field!)}")
                        .Aggregate((a, b) => $"{a},{b}");
                string command = $"CREATE STABLE IF NOT EXISTS {tableName} ({fieldNames}) TAGS (instance varchar(64));";
                tdEngineClient?.Exec(command);
            }
            catch (Exception e)
            {
                throw new TdEngineException($"upsert class time series {tableName}", e);
            }
            return tableName;
        }

        public void HistorizeValues(string repositoryStorageId, string classTimeSerieId, string instanceName, DateTime dateTime, List<IField> fieldsToHistorize)
        {
            string instanceTableName = instanceName.ToLowerInvariant();
            tdEngineClient!.Exec($"USE {repositoryStorageId};");
            using var stmt = tdEngineClient!.StmtInit();
            try
            {
                string fieldsPlaceholders = Enumerable.Repeat("?", fieldsToHistorize.Count + 1)
                    .Aggregate((a, b) => $"{a},{b}");
                string sql = $"INSERT INTO ? USING {classTimeSerieId} TAGS(?) VALUES ({fieldsPlaceholders});";
                List<object> rowValues = new(fieldsToHistorize.Count + 1)
                    {
                        dateTime
                    };
                fieldsToHistorize.ForEach(field =>
                    rowValues.Add(((dynamic)field).Value));
                stmt.Prepare(sql);
                // set table name
                stmt.SetTableName($"{instanceTableName}");
                // set tags
                stmt.SetTags([instanceTableName]);
                // bind row values
                stmt.BindRow([.. rowValues]);
                // add batch
                stmt.AddBatch();
                // execute
                stmt.Exec();
            }
            catch (Exception e)
            {
                throw new TdEngineException($"insert to table {classTimeSerieId}", e);
            }
        }

        public List<HistoryRow> GetHistoryValues(string repositoryStorageId, string classTimeSerieId, string instanceName, DateTime from, DateTime to, List<IFieldDefinition> fields)
        {
            string instanceTableName = instanceName.ToLowerInvariant();
            List<HistoryRow> rows = [];
            try
            {
                tdEngineClient!.Exec($"USE {repositoryStorageId};");
                string fieldNames = "TS," + fields.Select(field => $"_{field.Name}")
                    .Aggregate((a, b) => $"{a},{b}");
                string query = $"SELECT {fieldNames} FROM {instanceTableName} WHERE TS between {FormatToSqlDate(from)} and {FormatToSqlDate(to)}";
                using IRows row = tdEngineClient!.Query(query);
                while (row.Read())
                {
                    rows.Add(new HistoryRow(row));
                }
            }
            catch (Exception e)
            {
                throw new TdEngineException($"select from table {instanceTableName}", e);
            }
            return rows;
        }

        private static string FormatToSqlDate(DateTime dateTime)
        {
            return $"\"{dateTime:yyyy-MM-dd HH:mm:ss.fff}\"";
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            tdEngineClient?.Dispose();
        }

        private static string GetFieldDbType(IFieldDefinition field)
        {
            if (dotnetToDbTypes.TryGetValue(field.Type, out var dbType))
            {
                return dbType;
            }
            throw new UnhandledFieldTypeException(field.Name, field.Type);
        }
    }
}
