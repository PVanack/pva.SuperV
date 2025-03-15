using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Model.Instances;

namespace pva.SuperV.Model.HistoryRetrieval
{
    public static class HistoryRowMapper
    {
        public static List<HistoryRowModel> ToDto(List<HistoryRow> rows, List<IFieldDefinition> fields)
        {
            return [.. rows.Select(row
                => new HistoryRowModel(row.Ts.ToUniversalTime(), null, null, null, row.Quality, CreateRow(row, fields)
                    ))];
        }

        private static List<FieldValueModel> CreateRow(HistoryRow row, List<IFieldDefinition> fields)
        {
            List<FieldValueModel> rowValues = [];
            for (int index = 0; index < fields.Count; index++)
            {
                object rowValue = row.Values[index];
                IFieldDefinition field = fields[index];
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
                        FieldDefinition<string> derivedField => new StringFieldValueModel((string)rowValue, row.Quality, row.Ts),
                        FieldDefinition<TimeSpan> derivedField => new TimeSpanFieldValueModel((TimeSpan)rowValue, FieldValueMapper.FormatValue(derivedField, (TimeSpan)rowValue), row.Quality, row.Ts),
                        FieldDefinition<uint> derivedField => new UintFieldValueModel((uint)rowValue, FieldValueMapper.FormatValue(derivedField, (uint)rowValue), row.Quality, row.Ts),
                        FieldDefinition<ulong> derivedField => new UlongFieldValueModel((ulong)rowValue, FieldValueMapper.FormatValue(derivedField, (ulong)rowValue), row.Quality, row.Ts),
                        FieldDefinition<ushort> derivedField => new UshortFieldValueModel((ushort)rowValue, FieldValueMapper.FormatValue(derivedField, (ushort)rowValue), row.Quality, row.Ts),
                        _ => throw new UnhandledMappingException(nameof(FieldValueMapper), field.Type.ToString())
                    });
            }
            return rowValues;
        }

        public static List<HistoryRawRowModel> ToRawDto(List<HistoryRow> rows)
        {
            return [.. rows.Select(row
                => new HistoryRawRowModel(row.Ts.ToUniversalTime(), null, null, null, row.Quality,
                       [.. row.Values.Select(value => value)]
                    ))];
        }
    }
}
