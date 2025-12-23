using pva.SuperV.Engine.Processing;

namespace pva.SuperV.Engine.HistoryStorage
{
    public record InstanceTimeSerieParameters(List<IFieldDefinition> Fields, IHistorizationProcessing? HistorizationProcessing);
}
