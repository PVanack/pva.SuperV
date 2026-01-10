using System;
using System.Collections.Generic;
using System.Text;

namespace pva.SuperV.Engine
{
    public record FieldValueChangedEvent(
        string TopicName,
        IField Field,
        dynamic PreviousValue,
        dynamic NewValue);
}
