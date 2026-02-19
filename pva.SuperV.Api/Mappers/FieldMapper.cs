using pva.SuperV.Common;
using pva.SuperV.Engine;
using pva.SuperV.Model.Instances;

namespace pva.SuperV.Api.Mappers
{
    public static class FieldMapper
    {
        public static FieldModel ToDto(IField field)
               => new(field.FieldDefinition!.Name, field.Type.ToString(), FieldValueMapper.ToDto(field));
    }
}
