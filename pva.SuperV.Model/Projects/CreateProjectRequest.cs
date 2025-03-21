using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Projects
{
    [Description("Project creation reauest.")]
    [ExcludeFromCodeCoverage]
    public record CreateProjectRequest(
        [property: Description("Name of project")] string Name,
        [property: Description("Description of project")] string Description,
        [property: Description("Connection string to history storage")] string? HistoryStorageConnectionString = null)
    {
    }
}
