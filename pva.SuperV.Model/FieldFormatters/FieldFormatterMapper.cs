﻿using pva.SuperV.Engine;

namespace pva.SuperV.Model.FieldFormatters
{
    public static class FieldFormatterMapper
    {
        public static FieldFormatterModel? ToDto(FieldFormatter fieldFormatter)
        {
            if (fieldFormatter is EnumFormatter enumFormatter)
            {
                return new EnumFormatterModel(enumFormatter.Name!, enumFormatter.Values!);
            }
            return null;
        }

        public static FieldFormatter? FromDto(FieldFormatterModel fieldFormatterModel)
        {
            if (fieldFormatterModel is EnumFormatterModel enumFormatterModel)
            {
                return new EnumFormatter(enumFormatterModel.Name!, enumFormatterModel.Values!);
            }
            return null;
        }
    }
}
