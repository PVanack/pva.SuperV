using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Projects
{
    [Description("Project creation request.")]
    [ExcludeFromCodeCoverage]
    public record CreateProjectRequest(
        [property: Description("Name of project")]
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(Engine.Constants.IdentifierNamePattern, ErrorMessage = "Must be a valid identifier")]
        string Name,
        [property: Description("Description of project")]
        [Required]
        string Description,
        [property: Description("Connection string to history storage")]
        string? HistoryStorageConnectionString = null);
}
