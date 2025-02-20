using System.ComponentModel;

namespace pva.SuperV.Model.Projects
{
    public record CreateProjectRequest(
        [property: Description("Name of project")] string Name,
        [property: Description("Description of project")] string Description)
    {
    }
}
