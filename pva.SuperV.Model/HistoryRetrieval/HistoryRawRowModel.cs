﻿using pva.SuperV.Engine;
using System.ComponentModel;

namespace pva.SuperV.Model.HistoryRetrieval
{
    [Description("Row of field values from history")]
    public record HistoryRawRowModel(
        [property: Description("Timestamp of row.")]
        DateTime Timestamp,
        [property: Description("Quality level of row.")]
        QualityLevel Quality,
        [property: Description("Retrieved values.")]
        List<object> FieldValues)
    {
    }
}