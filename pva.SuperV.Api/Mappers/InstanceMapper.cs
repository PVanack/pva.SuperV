using pva.SuperV.Engine;
using pva.SuperV.Model.Instances;

namespace pva.SuperV.Api.Mappers
{
    public static class InstanceMapper
    {
        public static InstanceModel ToDto(IInstance instance)
            => new(instance.Name,
                instance.Class.Name,
                [.. instance.Fields.Values.Select(field => FieldMapper.ToDto(field))]);
    }
}
