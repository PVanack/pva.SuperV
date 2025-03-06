using pva.SuperV.Engine;

namespace pva.SuperV.Model.FieldDefinitions
{
    public static class FieldDefinitionMapper
    {
        public static IFieldDefinition FromDto(FieldDefinitionModel field)
        {
            return field switch
            {
                BoolFieldDefinitionModel derivedField => new FieldDefinition<bool>(field.Name, derivedField.DefaultValue),
                ShortFieldDefinitionModel derivedField => new FieldDefinition<short>(field.Name, derivedField.DefaultValue),
                UshortFieldDefinitionModel derivedField => new FieldDefinition<ushort>(field.Name, derivedField.DefaultValue),
                IntFieldDefinitionModel derivedField => new FieldDefinition<int>(field.Name, derivedField.DefaultValue),
                UintFieldDefinitionModel derivedField => new FieldDefinition<uint>(field.Name, derivedField.DefaultValue),
                LongFieldDefinitionModel derivedField => new FieldDefinition<long>(field.Name, derivedField.DefaultValue),
                UlongFieldDefinitionModel derivedField => new FieldDefinition<ulong>(field.Name, derivedField.DefaultValue),
                FloatFieldDefinitionModel derivedField => new FieldDefinition<float>(field.Name, derivedField.DefaultValue),
                DoubleFieldDefinitionModel derivedField => new FieldDefinition<double>(field.Name, derivedField.DefaultValue),
                StringFieldDefinitionModel derivedField => new FieldDefinition<string>(field.Name, derivedField.DefaultValue),
                DateTimeFieldDefinitionModel derivedField => new FieldDefinition<DateTime>(field.Name, derivedField.DefaultValue),
                TimeSpanFieldDefinitionModel derivedField => new FieldDefinition<TimeSpan>(field.Name, derivedField.DefaultValue),
                _ => throw new UnhandledMappingException(nameof(FieldDefinitionMapper), field?.Type.ToString()),
            };
        }

        public static FieldDefinitionModel ToDto(IFieldDefinition? field)
        {
            return field switch
            {
                FieldDefinition<bool> derivedField => new BoolFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name),
                FieldDefinition<short> derivedField => new ShortFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name),
                FieldDefinition<ushort> derivedField => new UshortFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name),
                FieldDefinition<int> derivedField => new IntFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name),
                FieldDefinition<uint> derivedField => new UintFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name),
                FieldDefinition<long> derivedField => new LongFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name),
                FieldDefinition<ulong> derivedField => new UlongFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name),
                FieldDefinition<float> derivedField => new FloatFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name),
                FieldDefinition<double> derivedField => new DoubleFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name),
                FieldDefinition<string> derivedField => new StringFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name),
                FieldDefinition<DateTime> derivedField => new DateTimeFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name),
                FieldDefinition<TimeSpan> derivedField => new TimeSpanFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name),
                _ => throw new UnhandledMappingException(nameof(FieldDefinitionMapper), field?.Type.ToString()),
            };
        }
    }
}