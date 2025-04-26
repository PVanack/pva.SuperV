using System.ComponentModel.DataAnnotations;

namespace pva.SuperV.Blazor.Components.Pages
{
    public class EditedProject(
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(Engine.Constants.IdentifierNamePattern, ErrorMessage = "Must be a valid identifier")]
        string name,
        [Required]
        string description,
        string? historyStorageConnectionString = null)
    {
        public EditedProject() : this("", "", null) { }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public string? HistoryStorageConnectionString
        {
            get => historyStorageConnectionString;
            set => historyStorageConnectionString = value;
        }
    }
}
