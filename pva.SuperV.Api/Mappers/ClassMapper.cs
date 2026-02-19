using pva.SuperV.Engine;
using pva.SuperV.Model.Classes;

namespace pva.SuperV.Api.Mappers
{
    public static class ClassMapper
    {
        public static ClassModel ToDto(Class clazz)
            => new(clazz.Name!, clazz.BaseClassName);
    }
}
