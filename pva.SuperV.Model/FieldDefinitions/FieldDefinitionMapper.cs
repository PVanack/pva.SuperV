using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Model.FieldDefinitions
{
    public static class FieldDefinitionMapper
    {
        public static IFieldDefinition FromDto(FieldDefinitionModel field)
        {
            return field switch
            {
                BoolFieldDefinitionModel derivedField => new FieldDefinition<bool>(field.Name, derivedField.DefaultValue, derivedField.TopicName),
                DateTimeFieldDefinitionModel derivedField => new FieldDefinition<DateTime>(field.Name, derivedField.DefaultValue, derivedField.TopicName),
                DoubleFieldDefinitionModel derivedField => new FieldDefinition<double>(field.Name, derivedField.DefaultValue, derivedField.TopicName),
                FloatFieldDefinitionModel derivedField => new FieldDefinition<float>(field.Name, derivedField.DefaultValue, derivedField.TopicName),
                IntFieldDefinitionModel derivedField => new FieldDefinition<int>(field.Name, derivedField.DefaultValue, derivedField.TopicName),
                LongFieldDefinitionModel derivedField => new FieldDefinition<long>(field.Name, derivedField.DefaultValue, derivedField.TopicName),
                ShortFieldDefinitionModel derivedField => new FieldDefinition<short>(field.Name, derivedField.DefaultValue, derivedField.TopicName),
                StringFieldDefinitionModel derivedField => new FieldDefinition<string>(field.Name, derivedField.DefaultValue, derivedField.TopicName),
                TimeSpanFieldDefinitionModel derivedField => new FieldDefinition<TimeSpan>(field.Name, derivedField.DefaultValue, derivedField.TopicName),
                UintFieldDefinitionModel derivedField => new FieldDefinition<uint>(field.Name, derivedField.DefaultValue, derivedField.TopicName),
                UlongFieldDefinitionModel derivedField => new FieldDefinition<ulong>(field.Name, derivedField.DefaultValue, derivedField.TopicName),
                UshortFieldDefinitionModel derivedField => new FieldDefinition<ushort>(field.Name, derivedField.DefaultValue, derivedField.TopicName),
                _ => throw new UnhandledMappingException(nameof(FieldDefinitionMapper), field?.FieldType),
            };
        }

        public static FieldDefinitionModel ToDto(IFieldDefinition? field)
        {
            return field switch
            {
                FieldDefinition<bool> derivedField => new BoolFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name, derivedField.TopicName),
                FieldDefinition<DateTime> derivedField => new DateTimeFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name, derivedField.TopicName),
                FieldDefinition<double> derivedField => new DoubleFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name, derivedField.TopicName),
                FieldDefinition<float> derivedField => new FloatFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name, derivedField.TopicName),
                FieldDefinition<int> derivedField => new IntFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name, derivedField.TopicName),
                FieldDefinition<long> derivedField => new LongFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name, derivedField.TopicName),
                FieldDefinition<short> derivedField => new ShortFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name, derivedField.TopicName),
                FieldDefinition<string> derivedField => new StringFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name, derivedField.TopicName),
                FieldDefinition<TimeSpan> derivedField => new TimeSpanFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name, derivedField.TopicName),
                FieldDefinition<uint> derivedField => new UintFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name, derivedField.TopicName),
                FieldDefinition<ulong> derivedField => new UlongFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name, derivedField.TopicName),
                FieldDefinition<ushort> derivedField => new UshortFieldDefinitionModel(field.Name, derivedField.DefaultValue, derivedField.Formatter?.Name, derivedField.TopicName),
                _ => throw new UnhandledMappingException(nameof(FieldDefinitionMapper), field?.Type.ToString()),
            };
        }
    }
}