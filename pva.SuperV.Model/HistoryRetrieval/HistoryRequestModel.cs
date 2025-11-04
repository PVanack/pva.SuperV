using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.HistoryRetrieval
{
    [Description("Request on fields' history")]
    [ExcludeFromCodeCoverage]
    public record HistoryRequestModel(
    [property:Description("Start time of request.")]
    DateTime StartTime,
    [property:Description("End time of request.")]
    DateTime EndTime,
    [property:Description("List of fields to be retrieved.")]
    List<string> HistoryFields);
}
