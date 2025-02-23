using pva.SuperV.Engine;

namespace pva.SuperV.Model.Classes
{
    public static class ClassMapper
    {
        public static ClassModel ToDto(Class clazz)
            => new(clazz.Name!, clazz.BaseClassName);
    }
}
