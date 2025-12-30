using NLog.Time;
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
        public async Task GivenFieldsWithHistory_WhenGettingHistoryRawValues_ThenHistoryRawRowsAreReturned()
        {
            // Given
            await BuildProjectAndCreateInstancesAsync();
            await TestFieldRawHistoryRetrieval<bool>(BoolFieldName, true);
            await TestFieldRawHistoryRetrieval<DateTime>(DateTimeFieldName, DateTime.Now);
            await TestFieldRawHistoryRetrieval<double>(DoubleFieldName, 123.456);
            await TestFieldRawHistoryRetrieval<float>(FloatFieldName, 12.345f);
            await TestFieldRawHistoryRetrieval<int>(IntFieldName, 123456);
            await TestFieldRawHistoryRetrieval<long>(LongFieldName, 654321);
            await TestFieldRawHistoryRetrieval<short>(ShortFieldName, 1234);
            await TestFieldRawHistoryRetrieval<string>(StringFieldName, "Hi from pva.SuperV!");
            await TestFieldRawHistoryRetrieval<TimeSpan>(TimeSpanFieldName, TimeSpan.FromDays(1));
            await TestFieldRawHistoryRetrieval<uint>(UintFieldName, 123);
            await TestFieldRawHistoryRetrieval<ulong>(UlongFieldName, 321456);
            await TestFieldRawHistoryRetrieval<ushort>(UshortFieldName, 1456);
            await TestFieldRawHistoryRetrieval<int>(IntFieldWithFormatName, 1);
        }

        [Fact]
        public async Task GivenFieldsWithHistory_WhenGettingHistoryValues_ThenHistoryRowsAreReturned()
        {
            // Given
            await BuildProjectAndCreateInstancesAsync();
            await TestFieldHistoryRetrieval<bool, BoolFieldValueModel>(BoolFieldName, true);
            await TestFieldHistoryRetrieval<DateTime, DateTimeFieldValueModel>(DateTimeFieldName, DateTime.Now);
            await TestFieldHistoryRetrieval<double, DoubleFieldValueModel>(DoubleFieldName, 123.456);
            await TestFieldHistoryRetrieval<float, FloatFieldValueModel>(FloatFieldName, 12.345f);
            await TestFieldHistoryRetrieval<int, IntFieldValueModel>(IntFieldName, 123456);
            await TestFieldHistoryRetrieval<long, LongFieldValueModel>(LongFieldName, 654321);
            await TestFieldHistoryRetrieval<short, ShortFieldValueModel>(ShortFieldName, 1234);
            await TestFieldHistoryRetrieval<string, StringFieldValueModel>(StringFieldName, "Hi from pva.SuperV!");
            await TestFieldHistoryRetrieval<TimeSpan, TimeSpanFieldValueModel>(TimeSpanFieldName, TimeSpan.FromDays(1));
            await TestFieldHistoryRetrieval<uint, UintFieldValueModel>(UintFieldName, 123);
            await TestFieldHistoryRetrieval<ulong, UlongFieldValueModel>(UlongFieldName, 321456);
            await TestFieldHistoryRetrieval<ushort, UshortFieldValueModel>(UshortFieldName, 1456);
            await TestFieldHistoryRetrieval<int, IntFieldValueModel>(IntFieldWithFormatName, 1, "High");
        }

        [Fact]
        public async Task GivenFieldsWithHistory_WhenGettingHistoryRawStatistics_ThenHistoryStatisticRawRowsAreReturned()
        {
            // Given
            await BuildProjectAndCreateInstancesAsync();
            await TestFieldRawHistoryStatsRetrieval<bool>(BoolFieldName, true);
            await TestFieldRawHistoryStatsRetrieval<DateTime>(DateTimeFieldName, DateTime.Now);
            await TestFieldRawHistoryStatsRetrieval<double>(DoubleFieldName, 123.456, HistoryStatFunction.AVG);
            await TestFieldRawHistoryStatsRetrieval<float>(FloatFieldName, 456.123f);
            await TestFieldRawHistoryStatsRetrieval<int>(IntFieldName, 456123);
            await TestFieldRawHistoryStatsRetrieval<long>(LongFieldName, 789456123);
            await TestFieldRawHistoryStatsRetrieval<short>(ShortFieldName, 26123);
            await TestFieldRawHistoryStatsRetrieval<string>(StringFieldName, "Hi from pva.SuperV!");
            //await TestFieldRawHistoryStatsRetrieval<TimeSpan>(TimeSpanFieldName, TimeSpan.FromDays(1));
            await TestFieldRawHistoryStatsRetrieval<uint>(UintFieldName, 78945);
            await TestFieldRawHistoryStatsRetrieval<ulong>(UlongFieldName, 54987);
            await TestFieldRawHistoryStatsRetrieval<ushort>(UshortFieldName, 33333);
        }

        [Fact]
        public async Task GivenFieldsWithHistory_WhenGettingHistoryStatistics_ThenHistoryStatisticRowsAreReturned()
        {
            // Given
            await BuildProjectAndCreateInstancesAsync();
            await TestFieldHistoryStatsRetrieval<bool, BoolFieldValueModel>(BoolFieldName, true);
            await TestFieldHistoryStatsRetrieval<DateTime, DateTimeFieldValueModel>(DateTimeFieldName, DateTime.Now);
            await TestFieldHistoryStatsRetrieval<double, DoubleFieldValueModel>(DoubleFieldName, 123.456, HistoryStatFunction.AVG);
            await TestFieldHistoryStatsRetrieval<float, FloatFieldValueModel>(FloatFieldName, 456.123f);
            await TestFieldHistoryStatsRetrieval<int, IntFieldValueModel>(IntFieldName, 456123);
            await TestFieldHistoryStatsRetrieval<long, LongFieldValueModel>(LongFieldName, 789456123);
            await TestFieldHistoryStatsRetrieval<short, ShortFieldValueModel>(ShortFieldName, 26123);
            await TestFieldHistoryStatsRetrieval<string, StringFieldValueModel>(StringFieldName, "Hi from pva.SuperV!");
            //await TestFieldHistoryStatsRetrieval<TimeSpan, TimeSpanFieldValueModel>(TimeSpanFieldName, TimeSpan.FromDays(1));
            await TestFieldHistoryStatsRetrieval<uint, UintFieldValueModel>(UintFieldName, 78945);
            await TestFieldHistoryStatsRetrieval<ulong, UlongFieldValueModel>(UlongFieldName, 54987);
            await TestFieldHistoryStatsRetrieval<ushort, UshortFieldValueModel>(UshortFieldName, 33333);
        }

        private async Task TestFieldHistoryRetrieval<TValue, TModel>(string fieldName, TValue value, string? formattedValue = null) where TModel : FieldValueModel
        {
            DateTime timestamp = DateTime.UtcNow;
            runnableProject!.SetInstanceValue<TValue>(AllFieldsInstanceName, fieldName, value, timestamp);
            TModel? model = (TModel?)Activator.CreateInstance(typeof(TModel), new object[] { value!, formattedValue, QualityLevel.Good, timestamp });
            HistoryResultModel expectedHistoryResult = new(
                [new HistoryFieldModel(fieldName, typeof(TValue).ToString(), 0)],
                [new HistoryRowModel(timestamp, QualityLevel.Good, [model!])]);

            // Act
            HistoryRequestModel request = new(timestamp.AddSeconds(-1), DateTime.Now, [fieldName]);
            HistoryResultModel historyResult = await historyValuesService.GetInstanceHistoryValuesAsync(runnableProject.GetId(), AllFieldsInstanceName, request);

            // Assert
            historyResult.ShouldBeEquivalentTo(expectedHistoryResult);
        }

        private async Task TestFieldRawHistoryRetrieval<T>(string fieldName, T value)
        {
            // Given
            DateTime timestamp = DateTime.UtcNow;
            runnableProject!.SetInstanceValue<T>(AllFieldsInstanceName, fieldName, value, timestamp);

            HistoryRawResultModel expectedHistoryResult = new(
                [new HistoryFieldModel(fieldName, value!.GetType().ToString(), 0)],
                [new HistoryRawRowModel(timestamp, QualityLevel.Good, [value])]);

            // Act
            HistoryRequestModel request = new(timestamp.AddSeconds(-1), DateTime.Now, [fieldName]);
            HistoryRawResultModel historyResult = await historyValuesService.GetInstanceRawHistoryValuesAsync(runnableProject.GetId(), AllFieldsInstanceName, request);

            // Assert
            CheckHistoryResult(expectedHistoryResult, historyResult);
        }

        private async Task TestFieldRawHistoryStatsRetrieval<T>(string fieldName, T value, HistoryStatFunction statisticalFunction = HistoryStatFunction.NONE)
        {
            DateTime timestamp = DateTime.UtcNow.Date;
            runnableProject!.SetInstanceValue<T>(AllFieldsInstanceName, fieldName, value, timestamp);
            HistoryStatisticsRawResultModel expectedHistoryResult = new(
                [
                    new HistoryStatisticResultFieldModel(fieldName, typeof(T).ToString(), 0, statisticalFunction)
                ],
                [
                    new HistoryStatisticsRawRowModel(timestamp, timestamp, timestamp.AddHours(1), TimeSpan.FromHours(1), QualityLevel.Good, [value!])
                ]);

            // Act
            HistoryStatisticsRequestModel request = new(timestamp, timestamp.AddHours(1), TimeSpan.FromHours(1), FillMode.PREV,
                [new HistoryStatisticFieldModel(fieldName, statisticalFunction)]);
            HistoryStatisticsRawResultModel historyResult = await historyValuesService.GetInstanceRawHistoryStatisticsAsync(runnableProject.GetId(), AllFieldsInstanceName, request);

            // Assert
            CheckHistoryStatisticsResult(expectedHistoryResult, historyResult);
        }

        private async Task TestFieldHistoryStatsRetrieval<TValue, TModel>(string fieldName, TValue value, HistoryStatFunction statisticalFunction = HistoryStatFunction.NONE) where TModel : FieldValueModel
        {
            DateTime timestamp = DateTime.UtcNow.Date;
            runnableProject!.SetInstanceValue<TValue>(AllFieldsInstanceName, fieldName, value, timestamp);
            TModel? model = (TModel?)Activator.CreateInstance(typeof(TModel), new object[] { value!, null, QualityLevel.Good, timestamp });
            HistoryStatisticsResultModel expectedHistoryResult = new(
                [new HistoryStatisticResultFieldModel(fieldName, typeof(TValue).ToString(), 0, statisticalFunction)],
                [new HistoryStatisticsRowModel(timestamp, timestamp, timestamp.AddHours(1), TimeSpan.FromHours(1), QualityLevel.Good,
                    [model!])
                ]);

            // Act
            HistoryStatisticsRequestModel request = new(timestamp, timestamp.AddMinutes(59), TimeSpan.FromHours(1), FillMode.PREV,
                [new HistoryStatisticFieldModel(fieldName, statisticalFunction)]);
            HistoryStatisticsResultModel historyResult = await historyValuesService.GetInstanceHistoryStatisticsAsync(runnableProject.GetId(), AllFieldsInstanceName, request);

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
