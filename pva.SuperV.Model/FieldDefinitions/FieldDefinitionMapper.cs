using pva.SuperV.Engine;

namespace pva.SuperV.Model.FieldDefinitions
{
    public static class FieldDefinitionMapper
    {
        public static IFieldDefinition FromDto(FieldDefinitionModel field)
        {
            return field?.Type switch
            {
                nameof(BoolFieldDefinitionModel) => new FieldDefinition<bool>(field.Name),
                nameof(ShortFieldDefinitionModel) => new FieldDefinition<short>(field.Name),
                nameof(UshortFieldDefinitionModel) => new FieldDefinition<ushort>(field.Name),
                nameof(IntFieldDefinitionModel) => new FieldDefinition<int>(field.Name),
                nameof(UintFieldDefinitionModel) => new FieldDefinition<uint>(field.Name),
                nameof(LongFieldDefinitionModel) => new FieldDefinition<long>(field.Name),
                nameof(UlongFieldDefinitionModel) => new FieldDefinition<ulong>(field.Name),
                nameof(FloatFieldDefinitionModel) => new FieldDefinition<float>(field.Name),
                nameof(DoubleFieldDefinitionModel) => new FieldDefinition<double>(field.Name),
                nameof(StringFieldDefinitionModel) => new FieldDefinition<string>(field.Name),
                nameof(DateTimeFieldDefinitionModel) => new FieldDefinition<DateTime>(field.Name),
                nameof(TimeSpanFieldDefinitionModel) => new FieldDefinition<TimeSpan>(field.Name),
                _ => throw new UnhandledMappingException(nameof(FieldDefinitionMapper), field?.Type.ToString()),
            };
        }

        public static FieldDefinitionModel ToDto(IFieldDefinition? field)
        {
            return field switch
            {
                FieldDefinition<bool> => new BoolFieldDefinitionModel(field.Name),
                FieldDefinition<short> => new ShortFieldDefinitionModel(field.Name),
                FieldDefinition<ushort> => new UshortFieldDefinitionModel(field.Name),
                FieldDefinition<int> => new IntFieldDefinitionModel(field.Name),
                FieldDefinition<uint> => new UintFieldDefinitionModel(field.Name),
                FieldDefinition<long> => new LongFieldDefinitionModel(field.Name!),
                FieldDefinition<ulong> => new UlongFieldDefinitionModel(field.Name),
                FieldDefinition<float> => new FloatFieldDefinitionModel(field.Name),
                FieldDefinition<double> => new DoubleFieldDefinitionModel(field.Name),
                FieldDefinition<string> => new StringFieldDefinitionModel(field.Name),
                FieldDefinition<DateTime> => new DateTimeFieldDefinitionModel(field.Name),
                FieldDefinition<TimeSpan> => new TimeSpanFieldDefinitionModel(field.Name),
                _ => throw new UnhandledMappingException(nameof(FieldDefinitionMapper), field?.Type.ToString()),
            };
        }
    }
}