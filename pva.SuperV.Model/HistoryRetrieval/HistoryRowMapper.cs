using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.HistoryRetrieval;
using pva.SuperV.Model.Instances;

namespace pva.SuperV.Model.HistoryRetrieval
{
    public static class HistoryRowMapper
    {
        public static List<HistoryRowModel> ToDto(List<HistoryRow> rows, List<IFieldDefinition> fields)
        {
            return [.. rows.Select(row
                => new HistoryRowModel(row.Ts.ToUniversalTime(), row.Quality, BuildRowValues(row, fields, false)
                    ))];
        }

        public static List<HistoryRawRowModel> ToRawDto(List<HistoryRow> rows)
        {
            return [.. rows.Select(row
                => new HistoryRawRowModel(row.Ts.ToUniversalTime(), row.Quality,
                       [.. row.Values.Select(value => value)]
                    ))];
        }

        public static List<HistoryStatisticsRowModel> ToDto(List<HistoryStatisticRow> rows, List<IFieldDefinition> fields)
        {
            return [.. rows.Select(row
                => new HistoryStatisticsRowModel(row.Ts.ToUniversalTime(), row.StartTime, row.EndTime, row.Duration, row.Quality, BuildRowValues(row, fields, true)
                    ))];
        }

        public static List<HistoryStatisticsRawRowModel> ToRawDto(List<HistoryStatisticRow> rows)
        {
            return [.. rows.Select(row
                => new HistoryStatisticsRawRowModel(row.Ts.ToUniversalTime(), row.StartTime, row.EndTime, row.Duration, row.Quality,
                       [.. row.Values.Select(value => value)]
                    ))];
        }

        private static List<FieldValueModel> BuildRowValues(HistoryRow row, List<IFieldDefinition> fields, bool useRowValuesDatatype)
        {
            List<FieldValueModel> rowValues = [];
            for (int index = 0; index < fields.Count; index++)
            {
                object? rowValue = row.Values[index];
                IFieldDefinition field = fields[index];
                if (useRowValuesDatatype)
                {
                    rowValues.Add(
                        rowValue switch
                        {
                            bool typedRowValue => new BoolFieldValueModel(typedRowValue, FieldValueMapper.FormatValue(field, typedRowValue), row.Quality, row.Ts),
                            DateTime typedRowValue => new DateTimeFieldValueModel(typedRowValue, FieldValueMapper.FormatValue(field, typedRowValue), row.Quality, row.Ts),
                            double typedRowValue => new DoubleFieldValueModel(typedRowValue, FieldValueMapper.FormatValue(field, typedRowValue), row.Quality, row.Ts),
                            float typedRowValue => new FloatFieldValueModel(typedRowValue, FieldValueMapper.FormatValue(field, typedRowValue), row.Quality, row.Ts),
                            int typedRowValue => new IntFieldValueModel(typedRowValue, FieldValueMapper.FormatValue(field, typedRowValue), row.Quality, row.Ts),
                            long typedRowValue => new LongFieldValueModel(typedRowValue, FieldValueMapper.FormatValue(field, typedRowValue), row.Quality, row.Ts),
                            short typedRowValue => new ShortFieldValueModel(typedRowValue, FieldValueMapper.FormatValue(field, typedRowValue), row.Quality, row.Ts),
                            string typedRowValue => new StringFieldValueModel(typedRowValue, row.Quality, row.Ts),
                            TimeSpan typedRowValue => new TimeSpanFieldValueModel(typedRowValue, FieldValueMapper.FormatValue(field, typedRowValue), row.Quality, row.Ts),
                            uint typedRowValue => new UintFieldValueModel(typedRowValue, FieldValueMapper.FormatValue(field, typedRowValue), row.Quality, row.Ts),
                            ulong typedRowValue => new UlongFieldValueModel(typedRowValue, FieldValueMapper.FormatValue(field, typedRowValue), row.Quality, row.Ts),
                            ushort typedRowValue => new UshortFieldValueModel(typedRowValue, FieldValueMapper.FormatValue(field, typedRowValue), row.Quality, row.Ts),
                            _ => throw new UnhandledMappingException(nameof(FieldValueMapper), field.Type.ToString())
                        });
                }
                else
                {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                    rowValues.Add(
                        field switch
                        {
                            FieldDefinition<bool> derivedField => new BoolFieldValueModel((bool)rowValue, FieldValueMapper.FormatValue(derivedField, (bool)rowValue), row.Quality, row.Ts),
                            FieldDefinition<DateTime> derivedField => new DateTimeFieldValueModel((DateTime)rowValue, FieldValueMapper.FormatValue(derivedField, (DateTime)rowValue), row.Quality, row.Ts),
                            FieldDefinition<double> derivedField => new DoubleFieldValueModel((double)rowValue, FieldValueMapper.FormatValue(derivedField, (double)rowValue), row.Quality, row.Ts),
                            FieldDefinition<float> derivedField => new FloatFieldValueModel((float)rowValue, FieldValueMapper.FormatValue(derivedField, (float)rowValue), row.Quality, row.Ts),
                            FieldDefinition<int> derivedField => new IntFieldValueModel((int)rowValue, FieldValueMapper.FormatValue(derivedField, (int)rowValue), row.Quality, row.Ts),
                            FieldDefinition<long> derivedField => new LongFieldValueModel((long)rowValue, FieldValueMapper.FormatValue(derivedField, (long)rowValue), row.Quality, row.Ts),
                            FieldDefinition<short> derivedField => new ShortFieldValueModel((short)rowValue, FieldValueMapper.FormatValue(derivedField, (short)rowValue), row.Quality, row.Ts),
                            FieldDefinition<string> => new StringFieldValueModel((string?)rowValue, row.Quality, row.Ts),
                            FieldDefinition<TimeSpan> derivedField => new TimeSpanFieldValueModel((TimeSpan)rowValue, FieldValueMapper.FormatValue(derivedField, (TimeSpan)rowValue), row.Quality, row.Ts),
                            FieldDefinition<uint> derivedField => new UintFieldValueModel((uint)rowValue, FieldValueMapper.FormatValue(derivedField, (uint)rowValue), row.Quality, row.Ts),
                            FieldDefinition<ulong> derivedField => new UlongFieldValueModel((ulong)rowValue, FieldValueMapper.FormatValue(derivedField, (ulong)rowValue), row.Quality, row.Ts),
                            FieldDefinition<ushort> derivedField => new UshortFieldValueModel((ushort)rowValue, FieldValueMapper.FormatValue(derivedField, (ushort)rowValue), row.Quality, row.Ts),
                            _ => throw new UnhandledMappingException(nameof(FieldValueMapper), field.Type.ToString())
                        });
#pragma warning restore CS8605 // Unboxing a possibly null value.
                }
            }
            return rowValues;
        }
    }
}
