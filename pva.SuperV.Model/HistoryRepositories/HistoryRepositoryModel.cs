using System.ComponentModel;

namespace pva.SuperV.Model.HistoryRepositories
{
    [Description("History repository")]
    public record HistoryRepositoryModel(
        [Description("Name of repository")] string Name)
    {
    }
}
