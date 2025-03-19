using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.HistoryRetrieval;
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
        /// TDengine history storage string.
        /// </summary>
        public const string Prefix = "TDengine";

        /// <summary>
        /// Contains the equivalence between .Net and TDengine data types for the types being handled.
        /// </summary>
        private static readonly Dictionary<Type, string> DotnetToDbTypes = new()
        {
            { typeof(DateTime), "TIMESTAMP" },
            { typeof(short), "SMALLINT"},
            { typeof(int), "INT" },
            { typeof(long), "BIGINT" },
            { typeof(uint), "INT UNSIGNED" },
            { typeof(ulong), "BIGINT UNSIGNED" },
            { typeof(float), "FLOAT" },
            { typeof(double),  "DOUBLE" },
            { typeof(bool), "BOOL" },
            { typeof(string), "NCHAR" },
            { typeof(sbyte),  "TINYINT" },
            { typeof(byte), "TINYINT UNSIGNED" },
            { typeof(ushort), "SMALLINT UNSIGNED" }
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
            connectionString = tdEngineConnectionString;
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
                throw new TdEngineException($"connect to {connectionString}", e);
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
                throw new TdEngineException($"upsert repository {repositoryName} on {connectionString}", e);
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
                throw new TdEngineException($"delete repository {repositoryActualName} on {connectionString}", e);
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
                string fieldNames = "TS TIMESTAMP, QUALITY NCHAR(10),";
                fieldNames +=
                    historizationProcessing.FieldsToHistorize
                        .Select(field => $"_{field.Name} {GetFieldDbType(field)}")
                        .Aggregate((a, b) => $"{a},{b}");
                string command = $"CREATE STABLE IF NOT EXISTS {tableName} ({fieldNames}) TAGS (instance varchar(64));";
                tdEngineClient?.Exec(command);
            }
            catch (SuperVException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new TdEngineException($"upsert class time series {tableName} on {connectionString}", e);
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
        public void HistorizeValues(string repositoryStorageId, string classTimeSerieId, string instanceName, DateTime timestamp, QualityLevel? quality, List<IField> fieldsToHistorize)
        {
            string instanceTableName = instanceName.ToLowerInvariant();
            tdEngineClient!.Exec($"USE {repositoryStorageId};");
            using var stmt = tdEngineClient!.StmtInit();
            try
            {
                string fieldsPlaceholders = Enumerable.Repeat("?", fieldsToHistorize.Count + 2)
                    .Aggregate((a, b) => $"{a},{b}");
                string sql = $"INSERT INTO ? USING {classTimeSerieId} TAGS(?) VALUES ({fieldsPlaceholders});";
                List<object> rowValues = new(fieldsToHistorize.Count + 2)
                    {
                        timestamp.ToLocalTime(),
                        (quality ?? QualityLevel.Good).ToString()
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
                throw new TdEngineException($"insert to table {classTimeSerieId} on {connectionString}", e);
            }
        }

        /// <summary>
        /// Gets instance values historized between 2 timestamps.
        /// </summary>
        /// <param name="repositoryStorageId">The history repository ID.</param>
        /// <param name="classTimeSerieId">The time series ID.</param>
        /// <param name="instanceName">The instance name.</param>
        /// <param name="timeRange">Time range for querying.</param>
        /// <param name="fields">List of fields to be retrieved. One of them should have the <see cref="HistorizationProcessing{T}"/></param>
        /// <returns>List of history rows.</returns>
        public List<HistoryRow> GetHistoryValues(string repositoryStorageId, string classTimeSerieId, string instanceName, HistoryTimeRange timeRange, List<IFieldDefinition> fields)
        {
            string instanceTableName = instanceName.ToLowerInvariant();
            List<HistoryRow> rows = [];
            try
            {
                tdEngineClient!.Exec($"USE {repositoryStorageId};");
                string fieldNames = fields.Select(field => $"_{field.Name}")
                    .Aggregate((a, b) => $"{a},{b}");
                string sqlQuery =
                    $@"
SELECT {fieldNames}, TS, QUALITY  FROM {instanceTableName}
 WHERE TS between ""{FormatToSqlDate(timeRange.From)}"" and ""{FormatToSqlDate(timeRange.To)}""
 ";
                using IRows row = tdEngineClient!.Query(sqlQuery);
                while (row.Read())
                {
                    rows.Add(new HistoryRow(row, fields));
                }
            }
            catch (Exception e)
            {
                throw new TdEngineException($"select from table {instanceTableName} on {connectionString}", e);
            }
            return rows;
        }

        /// <summary>
        /// Gets instance statistic values historized between 2 timestamps.
        /// </summary>
        /// <param name="repositoryStorageId">The history repository ID.</param>
        /// <param name="classTimeSerieId">The time series ID.</param>
        /// <param name="instanceName">The instance name.</param>
        /// <param name="timeRange">Query containing time range parameters.</param>
        /// <param name="fields">List of fields to be retrieved. One of them should have the <see cref="HistorizationProcessing{T}"/></param>
        /// <returns>List of history rows.</returns>
        public List<HistoryStatisticRow> GetHistoryStatistics(string repositoryStorageId, string classTimeSerieId, string instanceName,
            HistoryStatisticTimeRange timeRange, List<HistoryStatisticField> fields)
        {
            string instanceTableName = instanceName.ToLowerInvariant();
            List<HistoryStatisticRow> rows = [];
            try
            {
                tdEngineClient!.Exec($"USE {repositoryStorageId};");
                string fieldNames = fields.Select(field => $"{field.StatisticFunction}(_{field.Field.Name})")
                    .Aggregate((a, b) => $"{a},{b}");
                string fillClause = "";
                if (timeRange.FillMode is not null)
                {
                    fillClause = $"FILL({timeRange.FillMode})";
                }
                string sqlQuery =
                    $@"
SELECT {fieldNames}, _WSTART, _WEND, _WDURATION, _WSTART, MAX(QUALITY) FROM {instanceTableName}
 WHERE TS between ""{FormatToSqlDate(timeRange.From)}"" and ""{FormatToSqlDate(timeRange.To)}""
 INTERVAL({FormatInterval(timeRange.Interval)}) SLIDING({FormatInterval(timeRange.Interval)}) {fillClause}
 ";
                using IRows row = tdEngineClient!.Query(sqlQuery);
                while (row.Read())
                {
                    rows.Add(new HistoryStatisticRow(row, fields));
                }
            }
            catch (Exception e)
            {
                throw new TdEngineException($"select from table {instanceTableName} on {connectionString}", e);
            }
            return rows;
        }

        /// <summary>
        /// Formats a DateTime to SQL format used by TDengine.
        /// </summary>
        /// <param name="dateTime">The date time to be formatted.</param>
        /// <returns>SQL string for date time.</returns>
        private static string FormatToSqlDate(DateTime dateTime)
        {
            return $"{dateTime.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffK}";
        }

        private static string FormatInterval(TimeSpan interval)
        {
            TimeSpan timespan = interval;
            string intervalText = "";
            intervalText += GetIntervalPeriod(timespan.Days / 365, 'y');
            intervalText += GetIntervalPeriod((timespan.Days % 365) / 30, 'm');
            intervalText += GetIntervalPeriod(((timespan.Days % 365) % 30) / 7, 'w');
            intervalText += GetIntervalPeriod(((timespan.Days % 365) % 30) % 7, 'd');
            intervalText += GetIntervalPeriod(timespan.Hours, 'h');
            intervalText += GetIntervalPeriod(timespan.Minutes, 'm');
            intervalText += GetIntervalPeriod(timespan.Seconds, 's');
            intervalText += GetIntervalPeriod(timespan.Milliseconds, 'a');
            intervalText += GetIntervalPeriod(timespan.Nanoseconds, 'b');
            // Remove last comma and space
            return intervalText.TrimEnd()[..^1];
        }

        private static string GetIntervalPeriod(int value, char periodLetter)
        {
            if (value > 0)
            {
                return $"{value}{periodLetter}, ";
            }

            return "";
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
        /// <exception cref="UnhandledHistoryFieldTypeException"></exception>
        private static string GetFieldDbType(IFieldDefinition field)
        {
            if (DotnetToDbTypes.TryGetValue(field.Type, out var dbType))
            {
                return dbType;
            }
            throw new UnhandledHistoryFieldTypeException(field.Name, field.Type);
        }
    }
}
