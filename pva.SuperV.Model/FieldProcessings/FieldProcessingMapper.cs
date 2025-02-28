using pva.SuperV.Engine;
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
                        alarmStateProcessing.TrigerringFieldDefinition!.Type.ToString(),
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
                        historizationProcessing.TrigerringFieldDefinition!.Type.ToString(),
                        historizationProcessing.TrigerringFieldDefinition!.Name,
                        historizationProcessing.HistoryRepository!.Name,
                        historizationProcessing.TimestampFieldDefinition?.Name,
                        historizationProcessing.FieldsToHistorize.Select(field => field.Name).ToList()),
                _ => throw new UnhandledMappingException(nameof(FieldProcessingMapper), fieldProcessing.GetType().ToString()),
            };
        }

        public static IFieldValueProcessing FromDto(Project project, Class clazz, FieldValueProcessingModel fieldProcessingModel)
        {
            if (fieldProcessingModel is AlarmStateProcessingModel alarmStateProcessingModel)
            {
                return CreateAlarmStateProcessing(clazz, alarmStateProcessingModel);
            }
            else if (fieldProcessingModel is HistorizationProcessingModel historizationProcessingModel)
            {
                return CreateHistoryProcessing(project, clazz, fieldProcessingModel, historizationProcessingModel);
            }
            throw new UnhandledMappingException(nameof(FieldProcessingMapper), fieldProcessingModel.GetType().ToString());
        }

        private static IHistorizationProcessing CreateHistoryProcessing(Project project, Class clazz, FieldValueProcessingModel fieldProcessingModel, HistorizationProcessingModel historizationProcessingModel)
        {
            if (historizationProcessingModel.TrigerringFieldType == typeof(short).ToString())
            {
                return CreateHistorizationProcessing<short>(project, clazz, historizationProcessingModel);
            }
            else if (historizationProcessingModel.TrigerringFieldType == typeof(ushort).ToString())
            {
                return CreateHistorizationProcessing<ushort>(project, clazz, historizationProcessingModel);
            }
            else if (historizationProcessingModel.TrigerringFieldType == typeof(int).ToString())
            {
                return CreateHistorizationProcessing<int>(project, clazz, historizationProcessingModel);
            }
            else if (historizationProcessingModel.TrigerringFieldType == typeof(uint).ToString())
            {
                return CreateHistorizationProcessing<uint>(project, clazz, historizationProcessingModel);
            }
            else if (historizationProcessingModel.TrigerringFieldType == typeof(long).ToString())
            {
                return CreateHistorizationProcessing<long>(project, clazz, historizationProcessingModel);
            }
            else if (historizationProcessingModel.TrigerringFieldType == typeof(ulong).ToString())
            {
                return CreateHistorizationProcessing<ulong>(project, clazz, historizationProcessingModel);
            }
            else
            {
                throw new UnhandledMappingException(nameof(FieldProcessingMapper), fieldProcessingModel.GetType().ToString());
            }
        }

        private static HistorizationProcessing<T> CreateHistorizationProcessing<T>(Project project, Class clazz, HistorizationProcessingModel historizationProcessingModel)
        {
            return new HistorizationProcessing<T>(
                historizationProcessingModel.Name,
                project,
                clazz,
                historizationProcessingModel.TrigerringFieldName,
                historizationProcessingModel.HistoryRepositoryName,
                historizationProcessingModel.TimestampFieldName,
                historizationProcessingModel.FieldsToHistorize);
        }

        private static IAlarmStateProcessing CreateAlarmStateProcessing(Class clazz, AlarmStateProcessingModel alarmStateProcessingModel)
        {
            if (alarmStateProcessingModel.TrigerringFieldType == typeof(short).ToString())
            {
                return CreateAlarmState<short>(clazz, alarmStateProcessingModel);
            }
            else if (alarmStateProcessingModel.TrigerringFieldType == typeof(ushort).ToString())
            {
                return CreateAlarmState<ushort>(clazz, alarmStateProcessingModel);
            }
            else if (alarmStateProcessingModel.TrigerringFieldType == typeof(int).ToString())
            {
                return CreateAlarmState<int>(clazz, alarmStateProcessingModel);
            }
            else if (alarmStateProcessingModel.TrigerringFieldType == typeof(uint).ToString())
            {
                return CreateAlarmState<uint>(clazz, alarmStateProcessingModel);
            }
            else if (alarmStateProcessingModel.TrigerringFieldType == typeof(long).ToString())
            {
                return CreateAlarmState<long>(clazz, alarmStateProcessingModel);
            }
            else if (alarmStateProcessingModel.TrigerringFieldType == typeof(ulong).ToString())
            {
                return CreateAlarmState<ulong>(clazz, alarmStateProcessingModel);
            }
            else if (alarmStateProcessingModel.TrigerringFieldType == typeof(float).ToString())
            {
                return CreateAlarmState<float>(clazz, alarmStateProcessingModel);
            }
            else if (alarmStateProcessingModel.TrigerringFieldType == typeof(double).ToString())
            {
                return CreateAlarmState<double>(clazz, alarmStateProcessingModel);
            }
            else
            {
                throw new UnhandledMappingException(nameof(FieldProcessingMapper), alarmStateProcessingModel.TrigerringFieldType);
            }
        }

        private static AlarmStateProcessing<T> CreateAlarmState<T>(Class clazz, AlarmStateProcessingModel alarmStateProcessingModel) where T : INumber<T>
        {
            return new AlarmStateProcessing<T>(
                alarmStateProcessingModel.Name,
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
}
