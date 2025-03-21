using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.HistoryRepositories
{
    [Description("History repository")]
    [ExcludeFromCodeCoverage]
    public record HistoryRepositoryModel(
        [Description("Name of repository")] string Name)
    {
    }
}
