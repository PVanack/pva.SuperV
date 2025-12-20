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

        [Fact]
        public async Task GivenAllFieldsInstanceWithHistory_WhenGettingHistoryRawValues_ThenHistoryRowsAreReturned()
        {
            // Given
            await BuildProjectAndCreateInstancesAsync();
            DateTime timestamp = DateTime.UtcNow;
            runnableProject!.SetInstanceValue<bool>(AllFieldsInstanceName, BoolFieldName, true, timestamp);
            //runnableProject!.SetInstanceValue<DateTime>(AllFieldsInstanceName, DateTimeFieldName, timestamp, timestamp);
            runnableProject!.SetInstanceValue<double>(AllFieldsInstanceName, DoubleFieldName, 123.456, timestamp);
            runnableProject!.SetInstanceValue<float>(AllFieldsInstanceName, FloatFieldName, 12.345f, timestamp);
            runnableProject!.SetInstanceValue<int>(AllFieldsInstanceName, IntFieldName, 123456, timestamp);
            runnableProject!.SetInstanceValue<long>(AllFieldsInstanceName, LongFieldName, 654321, timestamp);
            runnableProject!.SetInstanceValue<short>(AllFieldsInstanceName, ShortFieldName, 1234, timestamp);
            runnableProject!.SetInstanceValue<string>(AllFieldsInstanceName, StringFieldName, "Hi from pva.SuperV!", timestamp);
            runnableProject!.SetInstanceValue<TimeSpan>(AllFieldsInstanceName, TimeSpanFieldName, TimeSpan.FromDays(1), timestamp);
            runnableProject!.SetInstanceValue<uint>(AllFieldsInstanceName, UintFieldName, 123, timestamp);
            runnableProject!.SetInstanceValue<ulong>(AllFieldsInstanceName, UlongFieldName, 321456, timestamp);
            runnableProject!.SetInstanceValue<ushort>(AllFieldsInstanceName, UshortFieldName, 1456, timestamp);
            runnableProject!.SetInstanceValue<int>(AllFieldsInstanceName, IntFieldWithFormatName, 1, timestamp);

            HistoryRawResultModel expectedHistoryResult = new(
                [
                    new HistoryFieldModel(BoolFieldName, "System.Boolean", 0),
                    //new HistoryFieldModel(DateTimeFieldName, "System.DateTime", 1),
                    new HistoryFieldModel(DoubleFieldName, "System.Double", 1),
                    new HistoryFieldModel(FloatFieldName, "System.Single", 2),
                    new HistoryFieldModel(IntFieldName, "System.Int32", 3),
                    new HistoryFieldModel(LongFieldName, "System.Int64", 4),
                    new HistoryFieldModel(ShortFieldName, "System.Int16", 5),
                    new HistoryFieldModel(StringFieldName, "System.String", 6),
                    new HistoryFieldModel(TimeSpanFieldName, "System.TimeSpan", 7),
                    new HistoryFieldModel(UintFieldName, "System.UInt32", 8),
                    new HistoryFieldModel(UlongFieldName, "System.UInt64", 9),
                    new HistoryFieldModel(UshortFieldName, "System.UInt16", 10),
                    new HistoryFieldModel(IntFieldWithFormatName, "System.Int32", 11),
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
                    BoolFieldName,
                    //DateTimeFieldName,
                    DoubleFieldName,
                    FloatFieldName,
                    IntFieldName,
                    LongFieldName,
                    ShortFieldName,
                    StringFieldName,
                    TimeSpanFieldName,
                    UintFieldName,
                    UlongFieldName,
                    UshortFieldName,
                    IntFieldWithFormatName
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
            HistoryResultModel expectedHistoryResult = new(
                [
                    new HistoryFieldModel(ValueFieldName, "System.Int32", 0)
                ],
                [
                    new HistoryRowModel(timestamp, QualityLevel.Good, [new IntFieldValueModel(123456, null, QualityLevel.Good, timestamp)])
                ]);

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
            runnableProject!.SetInstanceValue<bool>(AllFieldsInstanceName, BoolFieldName, true, timestamp);
            runnableProject!.SetInstanceValue<DateTime>(AllFieldsInstanceName, DateTimeFieldName, timestamp, timestamp);
            runnableProject!.SetInstanceValue<double>(AllFieldsInstanceName, DoubleFieldName, 123.456, timestamp);
            runnableProject!.SetInstanceValue<float>(AllFieldsInstanceName, FloatFieldName, 12.345f, timestamp);
            runnableProject!.SetInstanceValue<int>(AllFieldsInstanceName, IntFieldName, 123456, timestamp);
            runnableProject!.SetInstanceValue<long>(AllFieldsInstanceName, LongFieldName, 654321, timestamp);
            runnableProject!.SetInstanceValue<short>(AllFieldsInstanceName, ShortFieldName, 1234, timestamp);
            runnableProject!.SetInstanceValue<string>(AllFieldsInstanceName, StringFieldName, "Hi from pva.SuperV!", timestamp);
            runnableProject!.SetInstanceValue<TimeSpan>(AllFieldsInstanceName, TimeSpanFieldName, TimeSpan.FromDays(1), timestamp);
            runnableProject!.SetInstanceValue<uint>(AllFieldsInstanceName, UintFieldName, 123, timestamp);
            runnableProject!.SetInstanceValue<ulong>(AllFieldsInstanceName, UlongFieldName, 321456, timestamp);
            runnableProject!.SetInstanceValue<ushort>(AllFieldsInstanceName, UshortFieldName, 1456, timestamp);
            runnableProject!.SetInstanceValue<int>(AllFieldsInstanceName, IntFieldWithFormatName, 1, timestamp);

            HistoryResultModel expectedHistoryResult = new(
                [
                    new HistoryFieldModel(BoolFieldName, "System.Boolean", 0),
                    //new HistoryFieldModel(DateTimeFieldName, "System.DateTime", 1),
                    new HistoryFieldModel(DoubleFieldName, "System.Double", 1),
                    new HistoryFieldModel(FloatFieldName, "System.Single", 2),
                    new HistoryFieldModel(IntFieldName, "System.Int32", 3),
                    new HistoryFieldModel(LongFieldName, "System.Int64", 4),
                    new HistoryFieldModel(ShortFieldName, "System.Int16", 5),
                    new HistoryFieldModel(StringFieldName, "System.String", 6),
                    new HistoryFieldModel(TimeSpanFieldName, "System.TimeSpan", 7),
                    new HistoryFieldModel(UintFieldName, "System.UInt32", 8),
                    new HistoryFieldModel(UlongFieldName, "System.UInt64", 9),
                    new HistoryFieldModel(UshortFieldName, "System.UInt16", 10),
                    new HistoryFieldModel(IntFieldWithFormatName, "System.Int32", 11),
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
                    BoolFieldName,
                    //DateTimeFieldName,
                    DoubleFieldName,
                    FloatFieldName,
                    IntFieldName,
                    LongFieldName,
                    ShortFieldName,
                    StringFieldName,
                    TimeSpanFieldName,
                    UintFieldName,
                    UlongFieldName,
                    UshortFieldName,
                    IntFieldWithFormatName
                ]);
            HistoryResultModel historyResult = await historyValuesService.GetInstanceHistoryValuesAsync(runnableProject.GetId(), AllFieldsInstanceName, request);

            // Assert
            historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
        }

        [Fact]
        public async Task GivenInstanceWithHistory_WhenGettingHistoryRawStatistics_ThenHistoryStatisticRawRowsAreReturned()
        {
            // Given
            await BuildProjectAndCreateInstancesAsync();
            DateTime timestamp = DateTime.UtcNow.Date;
            runnableProject!.SetInstanceValue<int>(InstanceName, ValueFieldName, 123456, timestamp);
            HistoryStatisticsRawResultModel expectedHistoryResult = new(
                [
                    new HistoryStatisticResultFieldModel(ValueFieldName, "System.Double", 0, HistoryStatFunction.AVG)
                ],
                [
                    new HistoryStatisticsRawRowModel(timestamp, timestamp, timestamp.AddHours(1), TimeSpan.FromHours(1), QualityLevel.Good, [123456.0])
                ]);

            // Act
            HistoryStatisticsRequestModel request = new(timestamp, timestamp.AddHours(1), TimeSpan.FromHours(1), FillMode.PREV,
                [new HistoryStatisticFieldModel(ValueFieldName, HistoryStatFunction.AVG)]);
            HistoryStatisticsRawResultModel historyResult = await historyValuesService.GetInstanceRawHistoryStatisticsAsync(runnableProject.GetId(), InstanceName, request);

            // Assert
            CheckHistoryStatisticsResult(expectedHistoryResult, historyResult);
        }

        [Fact]
        public async Task GivenInstanceWithHistory_WhenGettingHistoryStatistics_ThenHistoryStatisticRowsAreReturned()
        {
            // Given
            await BuildProjectAndCreateInstancesAsync();
            DateTime timestamp = DateTime.UtcNow.Date;
            runnableProject!.SetInstanceValue<int>(InstanceName, ValueFieldName, 123456, timestamp);
            HistoryStatisticsResultModel expectedHistoryResult = new(
                [new HistoryStatisticResultFieldModel(ValueFieldName, "System.Double", 0, HistoryStatFunction.AVG)],
                [new HistoryStatisticsRowModel(timestamp, timestamp, timestamp.AddHours(1), TimeSpan.FromHours(1), QualityLevel.Good,
                    [new DoubleFieldValueModel(123456.0, null, QualityLevel.Good, timestamp)])
                ]);

            // Act
            HistoryStatisticsRequestModel request = new(timestamp, timestamp.AddMinutes(59), TimeSpan.FromHours(1), FillMode.PREV,
                [new HistoryStatisticFieldModel(ValueFieldName, HistoryStatFunction.AVG)]);
            HistoryStatisticsResultModel historyResult = await historyValuesService.GetInstanceHistoryStatisticsAsync(runnableProject.GetId(), InstanceName, request);

            // Assert
            historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
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
