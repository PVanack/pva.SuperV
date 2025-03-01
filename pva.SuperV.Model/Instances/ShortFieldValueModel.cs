﻿using pva.SuperV.Engine;

namespace pva.SuperV.Model.Instances
{
    public record ShortFieldValueModel(short Value, QualityLevel? Quality, DateTime? Timestamp) : FieldValueModel(Quality, Timestamp)
    {
    }
}
