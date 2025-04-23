using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using pva.SuperV.Blazor.SuperVClient;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class Project
    {
        [Parameter]
        public string ProjectId { get; set; } = default!;

        [Inject]
        private IRestClient SuperVRestClient { get; set; } = default!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        private State State { get; set; } = default!;

        private string pageTitle = default!;
        private bool success;
        private bool isModification;
        private CreateProjectRequest EditedProject { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            if (!String.IsNullOrEmpty(ProjectId))
            {
                State.EditedProject = await SuperVRestClient.GetProjectAsync(ProjectId);
            }
            if (State.EditedProject != null)
            {
                isModification = true;
                EditedProject.Name = State.EditedProject.Name;
                EditedProject.Description = State.EditedProject.Description;
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
                UpdateProjectRequest projectUpdate = new() { Description = EditedProject.Description, HistoryStorageConnectionString = EditedProject.HistoryStorageConnectionString };
                await SuperVRestClient.UpdateProjectAsync(State.EditedProject.Id, projectUpdate);
            }
            else
            {
                await SuperVRestClient.CreateBlankProjectAsync(EditedProject);
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
