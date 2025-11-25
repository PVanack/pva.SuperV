using pva.Helpers.Extensions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.HistoryRetrieval;
using pva.SuperV.Model.HistoryRetrieval;
using pva.SuperV.Model.Instances;
using Reqnroll.Assist;
using Shouldly;
using System.Net.Http.Json;
using System.Text.Json;

namespace pva.SuperV.TestsScenarios.StepDefinitions
{
    [Binding]
    public class HistoryValuesStepDefinitions(ScenarioContext scenarioContext) : BaseStepDefinition(scenarioContext)
    {
        [Then("Querying raw history values of instance {string} in project {string} between {string} and {string} returns fields history values")]
        public async ValueTask RawHistorizedFieldsShouldHaveExpectedValues(string instanceName, string projectId, string fromString, string toString, DataTable fields)
        {
            List<string> fieldNames = [];
            List<string> fieldActualTypes = [];
            Dictionary<string, string> fieldTypes = [];
            foreach (var header in fields.Header)
            {
                if (header != "Ts" && header != "Quality")
                {
                    string[] parts = header.Split(',');
                    fieldNames.Add(parts[0]);
                    fieldTypes.Add(header, parts[1]);
                    fieldActualTypes.Add(GetActualFieldType(parts[1]));
                }
            }
            HistoryRawResultModel expectedHistoryResult = new(BuildHistoryHeader(fieldNames, fieldActualTypes),
                BuildHistoryRawRowValues(fields.Rows, fieldTypes));
            HistoryRequestModel request = new(fromString.ParseDateTimeInvariant(), toString.ParseDateTimeInvariant(), fieldNames);
            var result = await Client.PostAsJsonAsync($"/history/{projectId}/{instanceName}/values/raw", request);

            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            HistoryRawResultModel? historyResult = await result.Content.ReadFromJsonAsync<HistoryRawResultModel>();
            CheckHistoryResult(expectedHistoryResult, historyResult);
        }

        private static void CheckHistoryResult(HistoryRawResultModel expectedHistoryResult, HistoryRawResultModel? historyResult)
        {
            // This doesn' work, as comparison of the object values use Object Equals().
            //historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
            // As a workaround, we compare each element :-(
            historyResult.ShouldNotBeNull();
            historyResult.Header.ShouldBeEquivalentTo(expectedHistoryResult.Header);
            historyResult.Rows.Count.ShouldBe(expectedHistoryResult.Rows.Count);
            for (int rowIndex = 0; rowIndex < expectedHistoryResult.Rows.Count; rowIndex++)
            {
                var actualRow = historyResult.Rows[rowIndex];
                var expectedRow = expectedHistoryResult.Rows[rowIndex];
                actualRow.Timestamp.ShouldBe(expectedRow.Timestamp);
                actualRow.Quality.ShouldBe(expectedRow.Quality);
                actualRow.FieldValues.Count.ShouldBe(expectedRow.FieldValues.Count);
                for (int fieldIndex = 0; fieldIndex < actualRow.FieldValues.Count; fieldIndex++)
                {
                    CheckRawValue(actualRow.FieldValues[fieldIndex], expectedRow.FieldValues[fieldIndex]);
                }
            }
        }

        [Then("Querying history values of instance {string} in project {string} between {string} and {string} returns fields history values")]
        public async ValueTask HistorizedFieldsShouldHaveExpectedValues(string instanceName, string projectId, string fromString, string toString, DataTable fields)
        {
            List<string> fieldNames = [];
            List<string> fieldActualTypes = [];
            Dictionary<string, string> fieldTypes = [];
            foreach (var header in fields.Header)
            {
                if (header != "Ts" && header != "Quality")
                {
                    string[] parts = header.Split(',');
                    fieldNames.Add(parts[0]);
                    fieldTypes.Add(header, parts[1]);
                    fieldActualTypes.Add(GetActualFieldType(parts[1]));
                }
            }
            HistoryResultModel expectedHistoryResult = new(BuildHistoryHeader(fieldNames, fieldActualTypes),
                BuildHistoryRowValues(fields.Rows, fieldTypes));
            HistoryRequestModel request = new(fromString.ParseDateTimeInvariant(), toString.ParseDateTimeInvariant(), fieldNames);
            var result = await Client.PostAsJsonAsync($"/history/{projectId}/{instanceName}/values", request);

            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            HistoryResultModel? historyResult = await result.Content.ReadFromJsonAsync<HistoryResultModel>();
            historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
        }

        [Then("Querying raw history statistics of instance {string} in project {string} between {string} and {string} with {string} interval returns fields statistic values")]
        public async ValueTask RawHistorizedFieldsShouldHaveExpectedStaticsticValues(string instanceName, string projectId, string fromString, string toString, string intervalString, DataTable fields)
        {
            List<string> fieldNames = [];
            List<HistoryStatisticFieldModel> fieldNamesWithFunction = [];
            List<HistoryStatFunction> fieldStatistics = [];
            List<string> fieldActualTypes = [];
            Dictionary<string, string> fieldTypes = [];
            foreach (var header in fields.Header)
            {
                if (header != "StartTs" && header != "EndTs" && header != "Quality")
                {
                    string[] parts = header.Split(',');
                    fieldNames.Add(parts[0]);
                    fieldTypes.Add(header, parts[1]);
                    fieldActualTypes.Add(GetActualFieldType(parts[1]));
                    HistoryStatFunction fieldStatistic = Enum.Parse<HistoryStatFunction>(parts[2]);
                    fieldStatistics.Add(fieldStatistic);
                    fieldNamesWithFunction.Add(new HistoryStatisticFieldModel(parts[0], fieldStatistic));
                }
            }
            HistoryStatisticsRawResultModel expectedHistoryResult = new(BuildHistoryStatisticsHeader(fieldNames, fieldActualTypes, fieldStatistics),
                BuildHistoryStatisticsRawRowValues(fields.Rows, fieldTypes));
            HistoryStatisticsRequestModel request = new(fromString.ParseDateTimeInvariant(), toString.ParseDateTimeInvariant(), intervalString.ParseTimeSpanInvariant(),
                FillMode.PREV, fieldNamesWithFunction);
            var result = await Client.PostAsJsonAsync($"/history/{projectId}/{instanceName}/statistics/raw", request);

            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            HistoryStatisticsRawResultModel? historyResult = await result.Content.ReadFromJsonAsync<HistoryStatisticsRawResultModel>();
            // This doesn' work, as comparison of the object values use Object Equals().
            //historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
            // As a workaround, we compare each element :-(
            historyResult.ShouldNotBeNull();
            historyResult.Header.ShouldBeEquivalentTo(expectedHistoryResult.Header);
            historyResult.Rows.Count.ShouldBe(expectedHistoryResult.Rows.Count);
            for (int rowIndex = 0; rowIndex < expectedHistoryResult.Rows.Count; rowIndex++)
            {
                var actualRow = historyResult.Rows[rowIndex];
                var expectedRow = expectedHistoryResult.Rows[rowIndex];
                actualRow.Timestamp.ShouldBe(expectedRow.Timestamp);
                actualRow.Quality.ShouldBe(expectedRow.Quality);
                actualRow.FieldValues.Count.ShouldBe(expectedRow.FieldValues.Count);
                for (int fieldIndex = 0; fieldIndex < actualRow.FieldValues.Count; fieldIndex++)
                {
                    CheckRawValue(actualRow.FieldValues[fieldIndex], expectedRow.FieldValues[fieldIndex]);
                }
            }
        }

        [Then("Querying history statistics of instance {string} in project {string} between {string} and {string} with {string} interval returns fields statistic values")]
        public async ValueTask HistorizedFieldsShouldHaveExpectedStaticsticValues(string instanceName, string projectId, string fromString, string toString, string intervalString, DataTable fields)
        {
            List<string> fieldNames = [];
            List<HistoryStatisticFieldModel> fieldNamesWithFunction = [];
            List<HistoryStatFunction> fieldStatistics = [];
            List<string> fieldActualTypes = [];
            Dictionary<string, string> fieldTypes = [];
            foreach (var header in fields.Header)
            {
                if (header != "StartTs" && header != "EndTs" && header != "Quality")
                {
                    string[] parts = header.Split(',');
                    fieldNames.Add(parts[0]);
                    fieldTypes.Add(header, parts[1]);
                    fieldActualTypes.Add(GetActualFieldType(parts[1]));
                    HistoryStatFunction fieldStatistic = Enum.Parse<HistoryStatFunction>(parts[2]);
                    fieldStatistics.Add(fieldStatistic);
                    fieldNamesWithFunction.Add(new HistoryStatisticFieldModel(parts[0], fieldStatistic));
                }
            }
            HistoryStatisticsResultModel expectedHistoryResult = new(BuildHistoryStatisticsHeader(fieldNames, fieldActualTypes, fieldStatistics),
                BuildHistoryStatisticsRowValues(fields.Rows, fieldTypes));
            HistoryStatisticsRequestModel request = new(fromString.ParseDateTimeInvariant(), toString.ParseDateTimeInvariant(), intervalString.ParseTimeSpanInvariant(),
                FillMode.PREV, fieldNamesWithFunction);
            var result = await Client.PostAsJsonAsync($"/history/{projectId}/{instanceName}/statistics", request);

            result.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            HistoryStatisticsResultModel? historyResult = await result.Content.ReadFromJsonAsync<HistoryStatisticsResultModel>();
            historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
        }

        private static void CheckRawValue(object actualValue, object expectedValue)
        {
            if (actualValue is null)
            {
                throw new ArgumentNullException(nameof(actualValue));
            }
            else if (expectedValue is null)
            {
                throw new ArgumentNullException(nameof(actualValue));
            }
            else
            {
                JsonElement? jsonElement = (JsonElement)actualValue;
                if (jsonElement.HasValue)
                {
                    object? actualTypedValue = expectedValue switch
                    {
                        bool => jsonElement.Value.GetBoolean(),
                        DateTime => jsonElement.Value.GetDateTime(),
                        double => jsonElement.Value.GetDouble(),
                        float => jsonElement.Value.GetSingle(),
                        int => jsonElement.Value.GetInt32(),
                        long => jsonElement.Value.GetInt64(),
                        short => jsonElement.Value.GetInt16(),
                        string => jsonElement.Value.GetString(),
                        TimeSpan => jsonElement!.Value.GetString()?.ParseTimeSpanInvariant(),
                        uint => jsonElement.Value.GetUInt32(),
                        ulong => jsonElement.Value.GetUInt64(),
                        ushort => jsonElement.Value.GetUInt16(),
                        _ => throw new NotImplementedException(),
                    };
                    actualTypedValue.ShouldBeEquivalentTo(expectedValue);
                }
            }
        }

        private static List<HistoryRowModel> BuildHistoryRowValues(DataTableRows rows, Dictionary<string, string> fieldTypes)
            => [.. rows.Select(row
                =>
                    {
                        DateTime rowTimestamp = row["Ts"].ParseDateTimeInvariant();
                        QualityLevel rowQuality = Enum.Parse<QualityLevel>(row["Quality"]);
                        return new HistoryRowModel(rowTimestamp, rowQuality,
                            [.. fieldTypes.Select(entry
                                => BuildHistoryValue(row, entry.Key, entry.Value, rowTimestamp, rowQuality))
                            ]);
                    })
                ];

        private static List<HistoryRawRowModel> BuildHistoryRawRowValues(DataTableRows rows, Dictionary<string, string> fieldTypes)
            => [.. rows.Select(row
                => new HistoryRawRowModel(row["Ts"].ParseDateTimeInvariant(), Enum.Parse<QualityLevel>(row["Quality"]),
                        [.. fieldTypes.Select(entry
                            => BuildHistoryRawValue(row, entry.Key, entry.Value))]
                        )
                )];

        private static FieldValueModel BuildHistoryValue(DataTableRow row, string cellName, string fieldType, DateTime rowTimestamp, QualityLevel rowQuality)
        {
            return BuildFieldValueModel(row, cellName, fieldType, null, rowQuality, rowTimestamp);
        }

        private static object BuildHistoryRawValue(DataTableRow row, string cellName, string fieldType)
        {
            return fieldType.ToLower() switch
            {
                "bool" => row.GetBoolean(cellName),
                "datetime" => row.GetDateTime(cellName),
                "double" => row.GetDouble(cellName),
                "float" => row.GetSingle(cellName),
                "int" => row.GetInt32(cellName),
                "long" => row.GetInt64(cellName),
                "short" => short.CreateChecked(row.GetInt32(cellName)),
                "string" => row[cellName],
                "timespan" => row[cellName].ParseTimeSpanInvariant(),
                "uint" => uint.CreateChecked(row.GetInt32(cellName)),
                "ulong" => ulong.CreateChecked(row.GetInt64(cellName)),
                "ushort" => ushort.CreateChecked(row.GetInt32(cellName)),
                _ => throw new NotImplementedException(),
            };
        }

        private static string GetActualFieldType(string fieldType)
        {
            return fieldType.ToLower() switch
            {
                "bool" => new bool().GetType().ToString(),
                "datetime" => DateTime.Now.GetType().ToString(),
                "double" => new double().GetType().ToString(),
                "float" => new float().GetType().ToString(),
                "int" => new int().GetType().ToString(),
                "long" => new long().GetType().ToString(),
                "short" => new short().GetType().ToString(),
                "string" => new string("").GetType().ToString(),
                "timespan" => new TimeSpan().GetType().ToString(),
                "uint" => new uint().GetType().ToString(),
                "ulong" => new ulong().GetType().ToString(),
                "ushort" => new ushort().GetType().ToString(),
                _ => throw new NotImplementedException(),
            };
        }

        private static List<HistoryFieldModel> BuildHistoryHeader(List<string> fieldNames, List<string> fieldTypes)
        {
            int index = 0;
            return [.. fieldNames.Select(fieldName
                        => new HistoryFieldModel(fieldName, fieldTypes[index], index++)
                    )];
        }
        private static List<HistoryStatisticsRawRowModel> BuildHistoryStatisticsRawRowValues(DataTableRows rows, Dictionary<string, string> fieldTypes)

            => [..rows.Select(row
                =>
                    {
                        DateTime startTs = row["StartTs"].ParseDateTimeInvariant();
                        DateTime endTs = row["EndTs"].ParseDateTimeInvariant();
                        return new HistoryStatisticsRawRowModel(startTs, startTs, endTs, endTs - startTs, Enum.Parse<QualityLevel>(row["Quality"]),
                                    [.. fieldTypes.Select(entry
                                        => BuildHistoryRawValue(row, entry.Key, entry.Value))]
                                    );
                    }
                )];

        private static List<HistoryStatisticResultFieldModel> BuildHistoryStatisticsHeader(List<string> fieldNames, List<string> fieldTypes, List<HistoryStatFunction> fieldStatistics)
        {
            int index = 0;
            return [.. fieldNames.Select(fieldName =>
                        {
                            int columnIndex = index;
                            index++;
                            return new HistoryStatisticResultFieldModel(fieldName, fieldTypes[columnIndex], columnIndex, fieldStatistics[columnIndex]);
                        }
                    )];
        }

        private static List<HistoryStatisticsRowModel> BuildHistoryStatisticsRowValues(DataTableRows rows, Dictionary<string, string> fieldTypes)
            => [.. rows.Select(row
                =>
                    {
                        DateTime startTs = row["StartTs"].ParseDateTimeInvariant();
                        DateTime endTs = row["EndTs"].ParseDateTimeInvariant();
                        DateTime rowTimestamp = row["StartTs"].ParseDateTimeInvariant();
                        QualityLevel rowQuality = Enum.Parse<QualityLevel>(row["Quality"]);
                        return new HistoryStatisticsRowModel(startTs, startTs, endTs, endTs - startTs, rowQuality,
                            [.. fieldTypes.Select(entry
                                => BuildHistoryValue(row, entry.Key, entry.Value, rowTimestamp, rowQuality))
                            ]);
                    })
                ];

    }
}
