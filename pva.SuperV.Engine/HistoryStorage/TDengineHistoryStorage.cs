using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.Processing;
using TDengine.Driver;
using TDengine.Driver.Client;

namespace pva.SuperV.Engine.HistoryStorage
{
    /// <summary>
    /// TDengine histiory storage engine.
    /// </summary>
    public class TDengineHistoryStorage : IHistoryStorageEngine
    {
        /// <summary>
        /// Contains the equivalence between .Net and TDengine data types for the types being handled.
        /// </summary>
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

        /// <summary>
        /// The connection string to the TDengine backend.
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// The TDengine clinet.
        /// </summary>
        private ITDengineClient? tdEngineClient;

        /// <summary>
        /// Builds a TDengine connection from connection stirng.
        /// </summary>
        /// <param name="tdEngineConnectionString">The TDengine connection string.</param>
        public TDengineHistoryStorage(string tdEngineConnectionString)
        {
            this.connectionString = tdEngineConnectionString;
            Connect();
        }

        /// <summary>
        /// Connects to TDengine.
        /// </summary>
        /// <exception cref="TdEngineException"></exception>
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

        /// <summary>
        /// Upsert a history repository in storage engine.
        /// </summary>
        /// <param name="projectName">Project name to zhich the repository belongs.</param>
        /// <param name="repository">History repository</param>
        /// <returns>ID of repository in storqge engine.</returns>
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

        /// <summary>
        /// Deletes a history repository from storage engine.
        /// </summary>
        /// <param name="projectName">Project name to zhich the repository belongs.</param>
        /// <param name="repositoryName">History repository name.</param>
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

        /// <summary>
        /// Upsert a class time series in storage engine
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repositoryStorageId">History respository in which the time series should be created.</param>
        /// <param name="projectName">Project name to zhich the time series belongs.</param>
        /// <param name="className">Class name</param>
        /// <param name="historizationProcessing">History processing for which the time series should be created.</param>
        /// <returns>Time series ID in storage engine.</returns>
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

        /// <summary>
        /// Historize instance values in storage engine
        /// </summary>
        /// <param name="repositoryStorageId">The history repository ID.</param>
        /// <param name="classTimeSerieId">The time series ID.</param>
        /// <param name="instanceName">The instance name.</param>
        /// <param name="timestamp">the timestamp of the values</param>
        /// <param name="fieldsToHistorize">List of fields to be historized.</param>
        public void HistorizeValues(string repositoryStorageId, string classTimeSerieId, string instanceName, DateTime timestamp, List<IField> fieldsToHistorize)
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
                        timestamp
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

        /// <summary>
        /// Gets instance values historized between 2 timestamps.
        /// </summary>
        /// <param name="repositoryStorageId">The history repository ID.</param>
        /// <param name="classTimeSerieId">The time series ID.</param>
        /// <param name="instanceName">The instance name.</param>
        /// <param name="from">From timestamp.</param>
        /// <param name="to">To timestamp.</param>
        /// <param name="fields">List of fields to be retrieved. One of them should have the <see cref="HistorizationProcessing{T}"/></param>
        /// <returns>List of history rows.</returns>
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

        /// <summary>
        /// Formats a DateTime to SAL format used by TDengine.
        /// </summary>
        /// <param name="dateTime">The date time to be formatted.</param>
        /// <returns>SQL string for date time.</returns>
        private static string FormatToSqlDate(DateTime dateTime)
        {
            return $"\"{dateTime:yyyy-MM-dd HH:mm:ss.fff}\"";
        }

        /// <summary>
        /// Disposes the instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the instance. Dispose the TDengine connection.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            tdEngineClient?.Dispose();
        }

        /// <summary>
        /// Gets the TDengine data type for a field definition.
        /// </summary>
        /// <param name="field">Field fr zhich the TDengine data type should be retrieved.</param>
        /// <returns>TDengine data type.</returns>
        /// <exception cref="UnhandledFieldTypeException"></exception>
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
