using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.FieldFormatters;
using pva.SuperV.Engine.FieldValueFormatters;
using pva.SuperV.Model.FieldDefinitions;

namespace pva.SuperV.Model.FieldFormatters
{
    public static class FieldFormatterMapper
    {
        public static FieldFormatterModel ToDto(FieldFormatter fieldFormatter)
            => fieldFormatter switch
            {
                EnumFormatter enumFormatter => new EnumFormatterModel(enumFormatter.Name!, enumFormatter.Values!),
                _ => throw new UnhandledMappingException(nameof(FieldDefinitionMapper), fieldFormatter.GetType().ToString()),
            };

        public static FieldFormatter FromDto(FieldFormatterModel fieldFormatterModel)
            => fieldFormatterModel switch
            {
                EnumFormatterModel enumFormatterModel => new EnumFormatter(enumFormatterModel.Name!, enumFormatterModel.Values!),
                _ => throw new UnhandledMappingException(nameof(FieldDefinitionMapper), fieldFormatterModel.GetType().ToString()),
            };
    }
}
