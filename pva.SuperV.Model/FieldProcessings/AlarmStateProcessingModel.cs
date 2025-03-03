using System.ComponentModel;

namespace pva.SuperV.Model.FieldProcessings
{
    [Description("Alarm state processing.")]
    public record AlarmStateProcessingModel(
        string Name,
        string TrigerringFieldName,
        [property: Description("High high limit field name.")]
        string? HighHighLimitFieldName,
        [property: Description("High limit field name.")]
        string HighLimitFieldName,
        [property: Description("Low limit field name.")]
        string LowLimitFieldName,
        [property: Description("Low low limit field name.")]
        string? LowLowLimitFieldName,
        [property: Description("Deadband field name.")]
        string? DeadbandFieldName,
        [property: Description("Alarm state field name.")]
        string AlarmStateFieldName,
        [property: Description("Acknowledgement field name.")]
        string? AckStateFieldName)
        : FieldValueProcessingModel(Name, TrigerringFieldName)
    {
    }
}
