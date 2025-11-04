using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using pva.SuperV.Model.Projects;
using pva.SuperV.Model.Services;
using System.ComponentModel.DataAnnotations;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class Project
    {
        [Inject]
        private IProjectService ProjectServiceClient { get; set; } = default!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        private State State { get; set; } = default!;

        [Parameter]
        public string ProjectId { get; set; } = default!;

        private string pageTitle = default!;
        private bool success;
        private bool isModification;
        private EditedProject EditedProject { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            isModification = !String.IsNullOrEmpty(ProjectId);
            EditedProject = new();
            if (isModification)
            {
                State.CurrentProject = await ProjectServiceClient.GetProjectAsync(ProjectId);
                EditedProject = new(State.CurrentProject.Name, State.CurrentProject.Description!);
                State.SetProjectBreadcrumb(State.CurrentProject);
            }
            pageTitle = isModification ? $"Project {EditedProject.Name}" : "New project";
            await base.OnParametersSetAsync();
        }

        private async Task OnValidSubmit(EditContext _)
        {
            success = true;
            if (isModification)
            {
                UpdateProjectRequest projectUpdate = new(EditedProject.Description, EditedProject.HistoryStorageConnectionString);
                await ProjectServiceClient.UpdateProjectAsync(State.CurrentProject!.Id, projectUpdate);
            }
            else
            {
                CreateProjectRequest projectCreation = new(EditedProject.Name, EditedProject.Description, EditedProject.HistoryStorageConnectionString);
                await ProjectServiceClient.CreateProjectAsync(projectCreation);
            }
            GoBackToProjects();
        }

        private void CancelSubmit()
        {
            GoBackToProjects();
        }

        private void GoBackToProjects()
        {
            State.CurrentProject = null;
            NavigationManager.NavigateTo("/projects");
        }
    }

    public class EditedProject(string name, string description, string? historyStorageConnectionString = null)
    {
        public EditedProject() : this("", "", null) { }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression(Engine.Constants.IdentifierNamePattern, ErrorMessage = "Must be a valid identifier")]
        public string Name { get => name; set => name = value; }

        [Required]
        public string Description { get => description; set => description = value; }

        public string? HistoryStorageConnectionString { get => historyStorageConnectionString; set => historyStorageConnectionString = value; }
    }
}
