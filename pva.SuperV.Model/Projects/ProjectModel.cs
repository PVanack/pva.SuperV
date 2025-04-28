using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Projects
{
    /// <summary>
    /// Project exchanged through the APIs.
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Version"></param>
    /// <param name="Description"></param>
    [Description("SuperV project")]
    [ExcludeFromCodeCoverage]
    public record ProjectModel(
        [property : Description("Id of the project.")]
        string Id,
        [property : Description("Name of the project.")]
        string Name,
        [property : Description("Version of the project.")]
        int Version,
        [property : Description("Description of the project.")]
        string? Description,
        [property: Description("Indicates if the project is runnable.")]
        bool Runnable,
        [property: Description("Indicates if the project has history enabled.")]
        bool HistoryEnabled);
}
