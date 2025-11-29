using pva.SuperV.Api.Services.History;
using pva.SuperV.Engine;
using pva.SuperV.Engine.HistoryRetrieval;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.HistoryRetrieval;
using pva.SuperV.Model.Instances;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class HistoryValueServiceTests : SuperVTestsBase
    {
        public class ConsoleWriter(ITestOutputHelper output) : StringWriter
        {
            public override void WriteLine(string? value) => output.WriteLine(value!);
        }

        private readonly HistoryValuesService historyValuesService;
        private WipProject? wipProject;
        private RunnableProject? runnableProject;


        public HistoryValueServiceTests(ITestOutputHelper output)
        {
            historyValuesService = new(LoggerFactory);
            Console.SetOut(new ConsoleWriter(output));
        }

        private async ValueTask BuildProjectAndCreateInstancesAsync()
        {
            wipProject = CreateWipProject(TDengineHistoryStorage.Prefix);
            runnableProject = await Project.BuildAsync(wipProject);
            runnableProject.CreateInstance(ClassName, InstanceName);
            runnableProject.CreateInstance(AllFieldsClassName, AllFieldsInstanceName);
        }

        [Fact]
        public async Task GivenInstanceWithHistory_WhenGettingHistoryRawValues_ThenHistoryRawRowsAreReturned()
        {
            // Given
            await BuildProjectAndCreateInstancesAsync();
            DateTime timestamp = DateTime.UtcNow;
            runnableProject!.SetInstanceValue<int>(InstanceName, ValueFieldName, 123456, timestamp);
            HistoryRawResultModel expectedHistoryResult = new(
                [
                    new HistoryFieldModel(ValueFieldName, "System.Int32", 0)
                ],
                [
                    new HistoryRawRowModel(timestamp, QualityLevel.Good,
                        [
                            123456
                        ])
                ]);

            // Act
            HistoryRequestModel request = new(timestamp.AddSeconds(-1), DateTime.Now, [ValueFieldName]);
            HistoryRawResultModel historyResult = await historyValuesService.GetInstanceRawHistoryValuesAsync(runnableProject.GetId(), InstanceName, request);

            // Assert
            CheckHistoryResult(expectedHistoryResult, historyResult);
        }

        private static void CheckHistoryResult(HistoryRawResultModel expectedHistoryResult, HistoryRawResultModel historyResult)
        {
            // This doesn' work, as comparison of the object values uses Object Equals().
            //historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
            // As a workaround, we compare each element :-(
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
                    CheckHistoryValue(actualRow.FieldValues[fieldIndex], expectedRow.FieldValues[fieldIndex]);
                }
            }
        }

        [Fact]
        public async Task GivenAllFieldsInstanceWithHistory_WhenGettingHistoryRawValues_ThenHistoryRowsAreReturned()
        {
            // Given
            await BuildProjectAndCreateInstancesAsync();
            DateTime timestamp = DateTime.UtcNow;
            runnableProject!.SetInstanceValue<bool>(AllFieldsInstanceName, "BoolField", true, timestamp);
            //runnableProject!.SetInstanceValue<DateTime>(AllFieldsInstanceName, "DateTimeField", timestamp, timestamp);
            runnableProject!.SetInstanceValue<double>(AllFieldsInstanceName, "DoubleField", 123.456, timestamp);
            runnableProject!.SetInstanceValue<float>(AllFieldsInstanceName, "FloatField", 12.345f, timestamp);
            runnableProject!.SetInstanceValue<int>(AllFieldsInstanceName, "IntField", 123456, timestamp);
            runnableProject!.SetInstanceValue<long>(AllFieldsInstanceName, "LongField", 654321, timestamp);
            runnableProject!.SetInstanceValue<short>(AllFieldsInstanceName, "ShortField", 1234, timestamp);
            runnableProject!.SetInstanceValue<string>(AllFieldsInstanceName, "StringField", "Hi from pva.SuperV!", timestamp);
            runnableProject!.SetInstanceValue<TimeSpan>(AllFieldsInstanceName, "TimeSpanField", TimeSpan.FromDays(1), timestamp);
            runnableProject!.SetInstanceValue<uint>(AllFieldsInstanceName, "UintField", 123, timestamp);
            runnableProject!.SetInstanceValue<ulong>(AllFieldsInstanceName, "UlongField", 321456, timestamp);
            runnableProject!.SetInstanceValue<ushort>(AllFieldsInstanceName, "UshortField", 1456, timestamp);
            runnableProject!.SetInstanceValue<int>(AllFieldsInstanceName, "IntFieldWithFormat", 1, timestamp);

            HistoryRawResultModel expectedHistoryResult = new(
                [
                    new HistoryFieldModel("BoolField", "System.Boolean", 0),
                    //new HistoryFieldModel("DateTimeField", "System.DateTime", 1),
                    new HistoryFieldModel("DoubleField", "System.Double", 1),
                    new HistoryFieldModel("FloatField", "System.Single", 2),
                    new HistoryFieldModel("IntField", "System.Int32", 3),
                    new HistoryFieldModel("LongField", "System.Int64", 4),
                    new HistoryFieldModel("ShortField", "System.Int16", 5),
                    new HistoryFieldModel("StringField", "System.String", 6),
                    new HistoryFieldModel("TimeSpanField", "System.TimeSpan", 7),
                    new HistoryFieldModel("UintField", "System.UInt32", 8),
                    new HistoryFieldModel("UlongField", "System.UInt64", 9),
                    new HistoryFieldModel("UshortField", "System.UInt16", 10),
                    new HistoryFieldModel("IntFieldWithFormat", "System.Int32", 11),
                ],
                [
                    new HistoryRawRowModel(timestamp, QualityLevel.Good,
                    [
                        true,
                        //new DateTimeFieldValueModel(timestamp, null, QualityLevel.Good, timestamp),
                        123.456,
                        12.345f,
                        123456,
                        654321L,
                        (short)1234,
                        "Hi from pva.SuperV!",
                        TimeSpan.FromDays(1),
                        (uint)123,
                        (ulong)321456,
                        (ushort)1456,
                        1,
                    ])
                ]);

            // Act
            HistoryRequestModel request = new(timestamp.AddSeconds(-1), DateTime.Now,
                [
                    "BoolField",
                    //"DateTimeField",
                    "DoubleField",
                    "FloatField",
                    "IntField",
                    "LongField",
                    "ShortField",
                    "StringField",
                    "TimeSpanField",
                    "UintField",
                    "UlongField",
                    "UshortField",
                    "IntFieldWithFormat"
                ]);
            HistoryRawResultModel historyResult = await historyValuesService.GetInstanceRawHistoryValuesAsync(runnableProject.GetId(), AllFieldsInstanceName, request);

            // Assert
            CheckHistoryResult(expectedHistoryResult, historyResult);
        }

        [Fact]
        public async Task GivenInstanceWithHistory_WhenGettingHistoryValues_ThenHistoryRowsAreReturned()
        {
            // Given
            await BuildProjectAndCreateInstancesAsync();
            DateTime timestamp = DateTime.UtcNow;
            runnableProject!.SetInstanceValue<int>(InstanceName, ValueFieldName, 123456, timestamp);
            HistoryResultModel expectedHistoryResult = new([new HistoryFieldModel(ValueFieldName, "System.Int32", 0)],
                [new HistoryRowModel(timestamp, QualityLevel.Good, [new IntFieldValueModel(123456, null, QualityLevel.Good, timestamp)])]);

            // Act
            HistoryRequestModel request = new(timestamp.AddSeconds(-1), DateTime.Now, [ValueFieldName]);
            HistoryResultModel historyResult = await historyValuesService.GetInstanceHistoryValuesAsync(runnableProject.GetId(), InstanceName, request);

            // Assert
            historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
        }

        [Fact]
        public async Task GivenAllFieldsInstanceWithHistory_WhenGettingHistoryValues_ThenHistoryRowsAreReturned()
        {
            // Given
            await BuildProjectAndCreateInstancesAsync();
            DateTime timestamp = DateTime.UtcNow;
            runnableProject!.SetInstanceValue<bool>(AllFieldsInstanceName, "BoolField", true, timestamp);
            runnableProject!.SetInstanceValue<DateTime>(AllFieldsInstanceName, "DateTimeField", timestamp, timestamp);
            runnableProject!.SetInstanceValue<double>(AllFieldsInstanceName, "DoubleField", 123.456, timestamp);
            runnableProject!.SetInstanceValue<float>(AllFieldsInstanceName, "FloatField", 12.345f, timestamp);
            runnableProject!.SetInstanceValue<int>(AllFieldsInstanceName, "IntField", 123456, timestamp);
            runnableProject!.SetInstanceValue<long>(AllFieldsInstanceName, "LongField", 654321, timestamp);
            runnableProject!.SetInstanceValue<short>(AllFieldsInstanceName, "ShortField", 1234, timestamp);
            runnableProject!.SetInstanceValue<string>(AllFieldsInstanceName, "StringField", "Hi from pva.SuperV!", timestamp);
            runnableProject!.SetInstanceValue<TimeSpan>(AllFieldsInstanceName, "TimeSpanField", TimeSpan.FromDays(1), timestamp);
            runnableProject!.SetInstanceValue<uint>(AllFieldsInstanceName, "UintField", 123, timestamp);
            runnableProject!.SetInstanceValue<ulong>(AllFieldsInstanceName, "UlongField", 321456, timestamp);
            runnableProject!.SetInstanceValue<ushort>(AllFieldsInstanceName, "UshortField", 1456, timestamp);
            runnableProject!.SetInstanceValue<int>(AllFieldsInstanceName, "IntFieldWithFormat", 1, timestamp);

            HistoryResultModel expectedHistoryResult = new(
                [
                    new HistoryFieldModel("BoolField", "System.Boolean", 0),
                    //new HistoryFieldModel("DateTimeField", "System.DateTime", 1),
                    new HistoryFieldModel("DoubleField", "System.Double", 1),
                    new HistoryFieldModel("FloatField", "System.Single", 2),
                    new HistoryFieldModel("IntField", "System.Int32", 3),
                    new HistoryFieldModel("LongField", "System.Int64", 4),
                    new HistoryFieldModel("ShortField", "System.Int16", 5),
                    new HistoryFieldModel("StringField", "System.String", 6),
                    new HistoryFieldModel("TimeSpanField", "System.TimeSpan", 7),
                    new HistoryFieldModel("UintField", "System.UInt32", 8),
                    new HistoryFieldModel("UlongField", "System.UInt64", 9),
                    new HistoryFieldModel("UshortField", "System.UInt16", 10),
                    new HistoryFieldModel("IntFieldWithFormat", "System.Int32", 11),
                ],
                [
                    new HistoryRowModel(timestamp, QualityLevel.Good,
                    [
                        new BoolFieldValueModel(true, null, QualityLevel.Good, timestamp),
                        //new DateTimeFieldValueModel(timestamp, null, QualityLevel.Good, timestamp),
                        new DoubleFieldValueModel(123.456, null, QualityLevel.Good, timestamp),
                        new FloatFieldValueModel(12.345f, null, QualityLevel.Good, timestamp),
                        new IntFieldValueModel(123456, null, QualityLevel.Good, timestamp),
                        new LongFieldValueModel(654321, null, QualityLevel.Good, timestamp),
                        new ShortFieldValueModel(1234, null, QualityLevel.Good, timestamp),
                        new StringFieldValueModel("Hi from pva.SuperV!", QualityLevel.Good, timestamp),
                        new TimeSpanFieldValueModel(TimeSpan.FromDays(1), null, QualityLevel.Good, timestamp),
                        new UintFieldValueModel(123, null, QualityLevel.Good, timestamp),
                        new UlongFieldValueModel(321456, null, QualityLevel.Good, timestamp),
                        new UshortFieldValueModel(1456, null, QualityLevel.Good, timestamp),
                        new IntFieldValueModel(1, "High", QualityLevel.Good, timestamp),
                    ])
                ]);

            // Act
            HistoryRequestModel request = new(timestamp.AddSeconds(-1), DateTime.Now,
                [
                    "BoolField",
                    //"DateTimeField",
                    "DoubleField",
                    "FloatField",
                    "IntField",
                    "LongField",
                    "ShortField",
                    "StringField",
                    "TimeSpanField",
                    "UintField",
                    "UlongField",
                    "UshortField",
                    "IntFieldWithFormat"
                ]);
            HistoryResultModel historyResult = await historyValuesService.GetInstanceHistoryValuesAsync(runnableProject.GetId(), AllFieldsInstanceName, request);

            // Assert
            historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
        }

        [Fact]
        public async Task GivenInstanceWithHistory_WhenGettingHistoryRawStatistics_ThenHistoryRawRowsAreReturned()
        {
            // Given
            await BuildProjectAndCreateInstancesAsync();
            DateTime timestamp = DateTime.UtcNow.Date;
            runnableProject!.SetInstanceValue<int>(InstanceName, ValueFieldName, 123456, timestamp);
            HistoryStatisticsRawResultModel expectedHistoryResult = new([new HistoryStatisticResultFieldModel(ValueFieldName, "System.Double", 0, HistoryStatFunction.AVG)],
                [new HistoryStatisticsRawRowModel(timestamp, timestamp, timestamp.AddHours(1), TimeSpan.FromHours(1), QualityLevel.Good, [123456.0])]);

            // Act
            HistoryStatisticsRequestModel request = new(timestamp, timestamp.AddHours(1), TimeSpan.FromHours(1), FillMode.PREV,
                [new HistoryStatisticFieldModel(ValueFieldName, HistoryStatFunction.AVG)]);
            HistoryStatisticsRawResultModel historyResult = await historyValuesService.GetInstanceRawHistoryStatisticsAsync(runnableProject.GetId(), InstanceName, request);

            // Assert
            CheckHistoryStatisticsResult(expectedHistoryResult, historyResult);
        }

        private static void CheckHistoryStatisticsResult(HistoryStatisticsRawResultModel expectedHistoryResult, HistoryStatisticsRawResultModel historyResult)
        {
            // This doesn' work, as comparison of the object values uses Object Equals().
            //historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
            // As a workaround, we compare each element :-(
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
                    CheckHistoryValue(actualRow.FieldValues[fieldIndex], expectedRow.FieldValues[fieldIndex]);
                }
            }
        }

        [Fact]
        public async Task GivenInstanceWithHistory_WhenGettingHistoryStatistics_ThenHistoryRowsAreReturned()
        {
            // Given
            await BuildProjectAndCreateInstancesAsync();
            DateTime timestamp = DateTime.UtcNow.Date;
            runnableProject!.SetInstanceValue<int>(InstanceName, ValueFieldName, 123456, timestamp);
            HistoryStatisticsResultModel expectedHistoryResult = new([new HistoryStatisticResultFieldModel(ValueFieldName, "System.Double", 0, HistoryStatFunction.AVG)],
                [new HistoryStatisticsRowModel(timestamp, timestamp, timestamp.AddHours(1), TimeSpan.FromHours(1), QualityLevel.Good,
                    [new DoubleFieldValueModel(123456.0, null, QualityLevel.Good, timestamp)])]);

            // Act
            HistoryStatisticsRequestModel request = new(timestamp, timestamp.AddMinutes(59), TimeSpan.FromHours(1), FillMode.PREV,
                [new HistoryStatisticFieldModel(ValueFieldName, HistoryStatFunction.AVG)]);
            HistoryStatisticsResultModel historyResult = await historyValuesService.GetInstanceHistoryStatisticsAsync(runnableProject.GetId(), InstanceName, request);

            // Assert
            historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
        }

        private static void CheckHistoryValue(object actualValue, object expectedValue)
        {
            actualValue.ShouldBeOfType(expectedValue.GetType());
            (actualValue switch
            {
                bool typedActual => new Action(() => typedActual.ShouldBe((bool)expectedValue)),
                DateTime typedActual => new Action(() => typedActual.ShouldBe((DateTime)expectedValue)),
                double typedActual => new Action(() => typedActual.ShouldBe((double)expectedValue)),
                float typedActual => new Action(() => typedActual.ShouldBe((float)expectedValue)),
                int typedActual => new Action(() => typedActual.ShouldBe((int)expectedValue)),
                long typedActual => new Action(() => typedActual.ShouldBe((long)expectedValue)),
                short typedActual => new Action(() => typedActual.ShouldBe((short)expectedValue)),
                string typedActual => new Action(() => typedActual.ShouldBe((string)expectedValue)),
                TimeSpan typedActual => new Action(() => typedActual.ShouldBe((TimeSpan)expectedValue)),
                uint typedActual => new Action(() => typedActual.ShouldBe((uint)expectedValue)),
                ulong typedActual => new Action(() => typedActual.ShouldBe((ulong)expectedValue)),
                ushort typedActual => new Action(() => typedActual.ShouldBe((ushort)expectedValue)),
                _ => throw new NotImplementedException(),
            })();
        }
    }
}
