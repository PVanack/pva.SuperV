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
            if (fieldProcessingModel is AlarmStateProcessingModel alarmStateProcessingModel)
            {
                return CreateAlarmStateProcessing(clazz, fieldDefinition, alarmStateProcessingModel);
            }
            else if (fieldProcessingModel is HistorizationProcessingModel historizationProcessingModel)
            {
                return CreateHistoryProcessing(project, clazz, fieldDefinition, historizationProcessingModel);
            }
            throw new UnhandledMappingException(nameof(FieldProcessingMapper), fieldDefinition.Type.ToString());
        }

        private static IHistorizationProcessing CreateHistoryProcessing(Project project, Class clazz, IFieldDefinition fieldDefinition, HistorizationProcessingModel historizationProcessingModel)
        {
            if (fieldDefinition.Type == typeof(bool))
            {
                return CreateHistorizationProcessing<bool>(project, clazz, historizationProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(DateTime))
            {
                return CreateHistorizationProcessing<DateTime>(project, clazz, historizationProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(double))
            {
                return CreateHistorizationProcessing<double>(project, clazz, historizationProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(float))
            {
                return CreateHistorizationProcessing<float>(project, clazz, historizationProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(int))
            {
                return CreateHistorizationProcessing<int>(project, clazz, historizationProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(long))
            {
                return CreateHistorizationProcessing<long>(project, clazz, historizationProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(short))
            {
                return CreateHistorizationProcessing<short>(project, clazz, historizationProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(string))
            {
                return CreateHistorizationProcessing<string>(project, clazz, historizationProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(TimeSpan))
            {
                return CreateHistorizationProcessing<TimeSpan>(project, clazz, historizationProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(uint))
            {
                return CreateHistorizationProcessing<uint>(project, clazz, historizationProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(ulong))
            {
                return CreateHistorizationProcessing<ulong>(project, clazz, historizationProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(ushort))
            {
                return CreateHistorizationProcessing<ushort>(project, clazz, historizationProcessingModel);
            }
            throw new UnhandledMappingException(nameof(FieldProcessingMapper), fieldDefinition.Type.ToString());
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
            if (fieldDefinition.Type == typeof(short))
            {
                return CreateAlarmState<short>(clazz, alarmStateProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(ushort))
            {
                return CreateAlarmState<ushort>(clazz, alarmStateProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(int))
            {
                return CreateAlarmState<int>(clazz, alarmStateProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(uint))
            {
                return CreateAlarmState<uint>(clazz, alarmStateProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(long))
            {
                return CreateAlarmState<long>(clazz, alarmStateProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(ulong))
            {
                return CreateAlarmState<ulong>(clazz, alarmStateProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(float))
            {
                return CreateAlarmState<float>(clazz, alarmStateProcessingModel);
            }
            else if (fieldDefinition.Type == typeof(double))
            {
                return CreateAlarmState<double>(clazz, alarmStateProcessingModel);
            }
            else
            {
                throw new UnhandledMappingException(nameof(FieldProcessingMapper), fieldDefinition.Type.ToString());
            }
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
