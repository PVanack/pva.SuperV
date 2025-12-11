using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.Processing;
using System.Numerics;

namespace pva.SuperV.Model.FieldProcessings
{
    public static class FieldProcessingMapper
    {
        public static FieldValueProcessingModel ToDto(IFieldValueProcessing fieldProcessing)
        {
            return fieldProcessing switch
            {
                IAlarmStateProcessing alarmStateProcessing =>
                    new AlarmStateProcessingModel(
                        alarmStateProcessing.Name,
                        alarmStateProcessing.TrigerringFieldDefinition!.Name,
                        alarmStateProcessing.HighHighLimitField?.Name,
                        alarmStateProcessing.HighLimitField!.Name,
                        alarmStateProcessing.LowLimitField!.Name,
                        alarmStateProcessing.LowLowLimitField?.Name,
                        alarmStateProcessing.DeadbandField?.Name,
                        alarmStateProcessing.AlarmStateField!.Name,
                        alarmStateProcessing.AckStateField?.Name),
                IHistorizationProcessing historizationProcessing =>
                    new HistorizationProcessingModel(
                        historizationProcessing.Name,
                        historizationProcessing.TrigerringFieldDefinition!.Name,
                        historizationProcessing.HistoryRepository!.Name,
                        historizationProcessing.TimestampFieldDefinition?.Name,
                        [.. historizationProcessing.FieldsToHistorize.Select(field => field.Name)]),
                _ => throw new UnhandledMappingException(nameof(FieldProcessingMapper), fieldProcessing.GetType().ToString()),
            };
        }

        public static IFieldValueProcessing FromDto(Project project, Class clazz, IFieldDefinition fieldDefinition, FieldValueProcessingModel fieldProcessingModel)
        {
            return fieldProcessingModel switch
            {
                AlarmStateProcessingModel alarmStateProcessingModel =>
                    CreateAlarmStateProcessing(clazz, fieldDefinition, alarmStateProcessingModel),
                HistorizationProcessingModel historizationProcessingModel =>
                    CreateHistoryProcessing(project, clazz, fieldDefinition, historizationProcessingModel),
                _ => throw new UnhandledMappingException(nameof(FieldProcessingMapper), fieldDefinition.Type.ToString())
            };
        }

        private static IHistorizationProcessing CreateHistoryProcessing(Project project, Class clazz, IFieldDefinition fieldDefinition, HistorizationProcessingModel historizationProcessingModel)
        {
            return fieldDefinition.Type switch
            {
                Type t when t == typeof(bool) => CreateHistorizationProcessing<bool>(project, clazz, historizationProcessingModel),
                Type t when t == typeof(DateTime) => CreateHistorizationProcessing<DateTime>(project, clazz, historizationProcessingModel),
                Type t when t == typeof(double) => CreateHistorizationProcessing<double>(project, clazz, historizationProcessingModel),
                Type t when t == typeof(float) => CreateHistorizationProcessing<float>(project, clazz, historizationProcessingModel),
                Type t when t == typeof(int) => CreateHistorizationProcessing<int>(project, clazz, historizationProcessingModel),
                Type t when t == typeof(long) => CreateHistorizationProcessing<long>(project, clazz, historizationProcessingModel),
                Type t when t == typeof(short) => CreateHistorizationProcessing<short>(project, clazz, historizationProcessingModel),
                Type t when t == typeof(string) => CreateHistorizationProcessing<string>(project, clazz, historizationProcessingModel),
                Type t when t == typeof(TimeSpan) => CreateHistorizationProcessing<TimeSpan>(project, clazz, historizationProcessingModel),
                Type t when t == typeof(uint) => CreateHistorizationProcessing<uint>(project, clazz, historizationProcessingModel),
                Type t when t == typeof(ulong) => CreateHistorizationProcessing<ulong>(project, clazz, historizationProcessingModel),
                Type t when t == typeof(ushort) => CreateHistorizationProcessing<ushort>(project, clazz, historizationProcessingModel),
                _ => throw new UnhandledMappingException(nameof(FieldProcessingMapper), fieldDefinition.Type.ToString()),
            };
        }

        private static HistorizationProcessing<T> CreateHistorizationProcessing<T>(Project project, Class clazz, HistorizationProcessingModel historizationProcessingModel)
            => new(historizationProcessingModel.Name,
                    project,
                    clazz,
                    historizationProcessingModel.TrigerringFieldName,
                    historizationProcessingModel.HistoryRepositoryName,
                    historizationProcessingModel.TimestampFieldName,
                    historizationProcessingModel.FieldsToHistorize);

        private static IAlarmStateProcessing CreateAlarmStateProcessing(Class clazz, IFieldDefinition fieldDefinition, AlarmStateProcessingModel alarmStateProcessingModel)
        {
            return fieldDefinition.Type switch
            {
                Type t when t == typeof(short) => CreateAlarmState<short>(clazz, alarmStateProcessingModel),
                Type t when t == typeof(ushort) => CreateAlarmState<ushort>(clazz, alarmStateProcessingModel),
                Type t when t == typeof(int) => CreateAlarmState<int>(clazz, alarmStateProcessingModel),
                Type t when t == typeof(uint) => CreateAlarmState<uint>(clazz, alarmStateProcessingModel),
                Type t when t == typeof(long) => CreateAlarmState<long>(clazz, alarmStateProcessingModel),
                Type t when t == typeof(ulong) => CreateAlarmState<ulong>(clazz, alarmStateProcessingModel),
                Type t when t == typeof(float) => CreateAlarmState<float>(clazz, alarmStateProcessingModel),
                Type t when t == typeof(double) => CreateAlarmState<double>(clazz, alarmStateProcessingModel),
                _ => throw new UnhandledMappingException(nameof(FieldProcessingMapper), fieldDefinition.Type.ToString())
            };
        }

        private static AlarmStateProcessing<T> CreateAlarmState<T>(Class clazz, AlarmStateProcessingModel alarmStateProcessingModel) where T : INumber<T>
            => new(alarmStateProcessingModel.Name,
                    clazz,
                    alarmStateProcessingModel.TrigerringFieldName,
                    alarmStateProcessingModel.HighHighLimitFieldName,
                    alarmStateProcessingModel.HighLimitFieldName,
                    alarmStateProcessingModel.LowLimitFieldName,
                    alarmStateProcessingModel.LowLowLimitFieldName,
                    alarmStateProcessingModel.DeadbandFieldName,
                    alarmStateProcessingModel.AlarmStateFieldName,
                    alarmStateProcessingModel.AckStateFieldName);
    }
}
