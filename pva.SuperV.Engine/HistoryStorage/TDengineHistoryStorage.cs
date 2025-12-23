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
            { typeof(TimeSpan), "BIGINT" },
            { typeof(uint), "INT UNSIGNED" },
            { typeof(ulong), "BIGINT UNSIGNED" },
            { typeof(float), "FLOAT" },
            { typeof(double),  "DOUBLE" },
            { typeof(bool), "BOOL" },
            { typeof(string), "NCHAR(132)" },
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
        /// <param name="projectName">Project name to hwich the repository belongs.</param>
        /// <param name="repository">History repository</param>
        /// <returns>ID of repository in storage engine.</returns>
        public string UpsertRepository(string projectName, HistoryRepository repository)
        {
            string repositoryName = GetRepositoryName(projectName, repository.Name);
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
            string repositoryActualName = GetRepositoryName(projectName, repositoryName);
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
        /// <param name="repositoryStorageId">History repository in which the time series should be created.</param>
        /// <param name="projectName">Project name to zhich the time series belongs.</param>
        /// <param name="className">Class name</param>
        /// <param name="historizationProcessing">History processing for which the time series should be created.</param>
        /// <returns>Time series ID in storage engine.</returns>
        public string UpsertClassTimeSerie<T>(string repositoryStorageId, string projectName, string className, HistorizationProcessing<T> historizationProcessing)
        {
            string classTimeSerieId = GetClassTimeSerieId(projectName, className, historizationProcessing);
            try
            {
                tdEngineClient?.Exec($"USE {repositoryStorageId};");
                string fieldNames = "TS TIMESTAMP, QUALITY NCHAR(10),";
                fieldNames +=
                    historizationProcessing.FieldsToHistorize
                        .Select(field => $"_{field.Name} {GetFieldDbType(field)}")
                        .Aggregate((a, b) => $"{a},{b}");
                string command = $"CREATE STABLE IF NOT EXISTS {classTimeSerieId} ({fieldNames}) TAGS (instance varchar(64));";
                tdEngineClient?.Exec(command);
            }
            catch (SuperVException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new TdEngineException($"upsert class time series {classTimeSerieId} on {connectionString}", e);
            }
            return classTimeSerieId;
        }

        /// <summary>
        /// Historize instance values in storage engine
        /// </summary>
        /// <param name="repositoryStorageId">The history repository ID.</param>
        /// <param name="historizationProcessingName">The historization processing name.</param>
        /// <param name="classTimeSerieId">The time series ID.</param>
        /// <param name="instanceName">The instance name.</param>
        /// <param name="timestamp">the timestamp of the values</param>
        /// <param name="quality">The quality level of the values.</param>
        /// <param name="fieldsToHistorize">List of fields to be historized.</param>
        public void HistorizeValues(string repositoryStorageId, string historizationProcessingName, string classTimeSerieId, string instanceName, DateTime timestamp, QualityLevel? quality, List<IField> fieldsToHistorize)
        {
            string instanceTableName = $"{instanceName}_{historizationProcessingName}".ToLowerInvariant();
            tdEngineClient!.Exec($"USE {repositoryStorageId};");
            using var stmt = tdEngineClient!.StmtInit();
            try
            {
                string fieldToHistorizeNames = fieldsToHistorize.Select(field => $"_{field.FieldDefinition!.Name}")
                    .Aggregate((a, b) => $"{a},{b}");
                string fieldValuesPlaceholders = Enumerable.Repeat("?", fieldsToHistorize.Count + 2)
                    .Aggregate((a, b) => $"{a},{b}");
                string sql = $@"INSERT INTO ? USING {classTimeSerieId} (instance) TAGS(?)
   (TS, QUALITY, {fieldToHistorizeNames}) VALUES ({fieldValuesPlaceholders});
";
                List<object> rowValues = new(fieldsToHistorize.Count + 2)
                {
                    timestamp.ToLocalTime(),
                    (quality ?? QualityLevel.Good).ToString()
                };
                fieldsToHistorize.ForEach(field =>
                    rowValues.Add(ConvertFieldValueToDb(field)));
                stmt.Prepare(sql);
                // set table name
                stmt.SetTableName(instanceTableName);
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
        /// <param name="instanceName">The instance name.</param>
        /// <param name="timeRange">Time range for querying.</param>
        /// <param name="instanceTimeSerieParameters">Parameters defining the time serie.</param>
        /// <param name="fields">List of fields to be retrieved. One of them should have the <see cref="HistorizationProcessing{T}"/></param>
        /// <returns>List of history rows.</returns>
        public List<HistoryRow> GetHistoryValues(string instanceName, HistoryTimeRange timeRange, InstanceTimeSerieParameters instanceTimeSerieParameters, List<IFieldDefinition> fields)
        {
            string instanceTableName = GetInstanceTableName(instanceName, instanceTimeSerieParameters);
            List<HistoryRow> rows = [];
            try
            {
                tdEngineClient!.Exec($"USE {instanceTimeSerieParameters.HistorizationProcessing!.HistoryRepository!.HistoryStorageId};");
                string fieldNames = fields.Select(field => $"_{field.Name}")
                    .Aggregate((a, b) => $"{a},{b}");
                string sqlQuery =
                    @$"
SELECT {fieldNames}, TS, QUALITY  FROM {instanceTableName}
 WHERE TS between ""{FormatToSqlDate(timeRange.From)}"" and ""{FormatToSqlDate(timeRange.To)}"";
 ";
                using IRows row = tdEngineClient!.Query(sqlQuery);
                while (row.Read())
                {
                    rows.Add(new HistoryRow(row, fields, true));
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
        /// <param name="instanceName">The instance name.</param>
        /// <param name="timeRange">Query containing time range parameters.</param>
        /// <param name="instanceTimeSerieParameters">Parameters defining the time serie.</param>
        /// <param name="fields">List of fields to be retrieved. One of them should have the <see cref="HistorizationProcessing{T}"/></param>
        /// <returns>List of history rows.</returns>
        public List<HistoryStatisticRow> GetHistoryStatistics(string instanceName, HistoryStatisticTimeRange timeRange,
            InstanceTimeSerieParameters instanceTimeSerieParameters, List<HistoryStatisticField> fields)
        {
            string instanceTableName = GetInstanceTableName(instanceName, instanceTimeSerieParameters);
            List<HistoryStatisticRow> rows = [];
            try
            {
                tdEngineClient!.Exec($"USE {instanceTimeSerieParameters.HistorizationProcessing!.HistoryRepository!.HistoryStorageId};");
                string fieldNames = fields.Select(field => $"{field.StatisticFunction}(_{field.Field.Name})")
                    .Aggregate((a, b) => $"{a},{b}");
                string fillClause = "";
                if (timeRange.FillMode is not null)
                {
                    fillClause = $"FILL({timeRange.FillMode})";
                }
                string sqlQuery =
                    @$"
SELECT {fieldNames}, _WSTART, _WEND, _WDURATION, _WSTART, MAX(QUALITY) FROM {instanceTableName}
 WHERE TS between ""{FormatToSqlDate(timeRange.From)}"" and ""{FormatToSqlDate(timeRange.To)}""
 INTERVAL({FormatInterval(timeRange.Interval)}) SLIDING({FormatInterval(timeRange.Interval)}) {fillClause};
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
        /// <param name="field">Field for which the TDengine data type should be retrieved.</param>
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

        private static string GetRepositoryName(string projectName, string repositoryName)
            => $"{projectName}{repositoryName}".ToLowerInvariant();

        private static string GetClassTimeSerieId<T>(string projectName, string className, HistorizationProcessing<T> historizationProcessing)
            => $"{projectName}_{className}_{historizationProcessing.Name}".ToLowerInvariant();

        private static string GetInstanceTableName(string instanceName, InstanceTimeSerieParameters instanceTimeSerieParameters)
            => $"{instanceName}_{instanceTimeSerieParameters!.HistorizationProcessing!.Name}".ToLowerInvariant();

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
            intervalText += GetIntervalPeriod((timespan.Days % 365 % 30) / 7, 'w');
            intervalText += GetIntervalPeriod(timespan.Days % 365 % 30 % 7, 'd');
            intervalText += GetIntervalPeriod(timespan.Hours, 'h');
            intervalText += GetIntervalPeriod(timespan.Minutes, 'm');
            intervalText += GetIntervalPeriod(timespan.Seconds, 's');
            intervalText += GetIntervalPeriod(timespan.Milliseconds, 'a');
            intervalText += GetIntervalPeriod(timespan.Nanoseconds, 'b');
            // Remove last comma and space
            return intervalText.TrimEnd()[..^1];
        }

        private static string GetIntervalPeriod(int value, char periodLetter)
            => value > 0 ? $"{value}{periodLetter}, " : "";

        private static object ConvertFieldValueToDb(IField field)
            => field switch
            {
                Field<bool> typedField => typedField.Value,
                Field<DateTime> typedField => typedField.Value.ToLocalTime(),
                Field<double> typedField => typedField.Value,
                Field<float> typedField => typedField.Value,
                Field<int> typedField => typedField.Value,
                Field<long> typedField => typedField.Value,
                Field<short> typedField => typedField.Value,
                Field<string> typedField => typedField.Value,
                Field<TimeSpan> typedField => typedField.Value.Ticks,
                Field<uint> typedField => typedField.Value,
                Field<ulong> typedField => typedField.Value,
                Field<ushort> typedField => typedField.Value,
                _ => throw new UnhandledMappingException(nameof(TDengineHistoryStorage), field.Type.ToString())
            };

    }
}
