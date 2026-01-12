namespace pva.SuperV.Engine
{
    public record FieldValueChangedEvent(
        string TopicName,
        IField Field,
        dynamic PreviousValue,
        dynamic NewValue);
}
