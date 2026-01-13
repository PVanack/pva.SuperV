namespace pva.SuperV.Engine.Processing
{
    public record FieldValueChangedEvent(
        string TopicName,
        IField Field,
        dynamic PreviousValue,
        dynamic NewValue);
}
