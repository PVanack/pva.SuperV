using pva.SuperV.Engine;

namespace pva.SuperV.Model.Instances
{
    public static class InstanceMapper
    {
        public static InstanceModel ToDto(IInstance instance)
            => new(instance.Name,
                instance.Class.Name!,
                [.. instance.Fields.Values.Select(field => FieldMapper.ToDto(field))]);
    }
}
