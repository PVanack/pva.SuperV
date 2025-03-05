using pva.SuperV.Engine;

namespace pva.SuperV.Model.Instances
{
    public static class FieldMapper
    {
        public static FieldModel ToDto(IField field)
               => new(field.FieldDefinition!.Name, field.Type.ToString(), FieldValueMapper.ToDto(field));
    }
}
