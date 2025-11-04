using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Projects
{
    [Description("Project update request.")]
    [ExcludeFromCodeCoverage]
    public record UpdateProjectRequest(
        [property: Description("Description of project")]
        string Description,
        [property: Description("Connection string to history storage")]
        string? HistoryStorageConnectionString = null);
}
