using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using pva.SuperV.Model.Projects;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class Project
    {
        [Parameter]
        public string ProjectId { get; set; } = default!;

        [Inject]
        private IProjectService ProjectServiceClient { get; set; } = default!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        private State State { get; set; } = default!;

        private string pageTitle = default!;
        private bool success;
        private bool isModification;
        private EditedProject EditedProject { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            if (!String.IsNullOrEmpty(ProjectId))
            {
                State.EditedProject = await ProjectServiceClient.GetProjectAsync(ProjectId);
            }
            if (State.EditedProject != null)
            {
                isModification = true;
                EditedProject = new(State.EditedProject.Name, State.EditedProject.Description!);
            }
            pageTitle = isModification ? $"Project {EditedProject.Name}" : "New project";
            State.AddProjectBreadcrumb(State.EditedProject);
            await base.OnParametersSetAsync();
        }

        private async Task OnValidSubmit(EditContext context)
        {
            success = true;
            if (State.EditedProject != null)
            {
                UpdateProjectRequest projectUpdate = new(EditedProject.Description, EditedProject.HistoryStorageConnectionString);
                await ProjectServiceClient.UpdateProjectAsync(State.EditedProject.Id, projectUpdate);
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
            NavigationManager.NavigateTo("/projects");
            State.EditedProject = null;
            State.RemoveLastBreadcrumb();
        }

    }
}
