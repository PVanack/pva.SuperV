﻿using pva.SuperV.Engine;

namespace pva.SuperV.Model
{
    public static class ClassMapper
    {
        public static ClassModel ToDto(Class clazz)
        {
            return new ClassModel(clazz.Name!, clazz.BaseClassName);
        }
    }
}
