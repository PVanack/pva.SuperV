using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using pva.SuperV.Blazor.Components.Dialogs;
using pva.SuperV.Blazor.SuperVClient;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class Projects
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        private IRestClient SuperVRestClient { get; set; } = default!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        private IDialogService DialogService { get; set; } = default!;
        [Inject]
        private State State { get; set; } = default!;

        private MudTable<ProjectModel> projectsTable = default!;
        private string projectNameSearchString = default!;
        private Dictionary<String, MudMenu?> ContextMenusRef { get; set; } = [];

        protected override void OnInitialized()
        {
            State.SetProjectsBreadcrumb();
            base.OnInitialized();
        }

        private async Task<TableData<ProjectModel>> ServerReload(TableState state, CancellationToken token)
        {
            ProjectPagedSearchRequest request = new() { PageNumber = state.Page + 1, PageSize = state.PageSize, NameFilter = projectNameSearchString };
            PagedSearchResultOfProjectModel projects = await SuperVRestClient.SearchProjectsAsync(request, token);
            ContextMenusRef.Clear();
            foreach (var project in projects.Result)
            {
                ContextMenusRef.Add(project.Id, null);
            }

            TableData<ProjectModel> projectsTableData = new() { TotalItems = projects.Count, Items = projects.Result };
            return projectsTableData;
        }

        private async Task CreateProject(MouseEventArgs e)
        {
            State.EditedProject = null;
            NavigationManager.NavigateTo("/project");
            await projectsTable.ReloadServerData();
        }

        private void RowClickedEvent(TableRowClickEventArgs<ProjectModel> tableRowClickEventArgs)
        {
            State.EditedProject = tableRowClickEventArgs.Item!;
            NavigationManager.NavigateTo("/project");
        }

        private async Task Search(string args)
        {
            await projectsTable.ReloadServerData();
        }

        private async Task CreateWipProjectFromRunnable(string runnableProjectId)
        {
            await SuperVRestClient.CreateProjectFromRunnableAsync(runnableProjectId);
            await projectsTable.ReloadServerData();
        }

        private async Task BuildWipProject(string wipProjectId)
        {
            await SuperVRestClient.BuildProjectAsync(wipProjectId);
            await projectsTable.ReloadServerData();
        }
        private async Task DeleteProject(string projectId)
        {
            var parameters = new DialogParameters<DeleteConfirmationDialog> { { x => x.EntityDescription, $"project {projectId}" } };

            var dialog = await DialogService.ShowAsync<DeleteConfirmationDialog>($"Delete project", parameters);
            var result = await dialog.Result;

            if (result is not null && !result.Canceled)
            {
                await SuperVRestClient.UnloadProjectAsync(projectId);
                await projectsTable.ReloadServerData();
            }
        }

        private async Task SaveProjectDefinitions(ProjectModel project)
        {
            string json = await SuperVRestClient.SaveProjectDefinitionsAsync(project.Id);
            string fileName = $"{project.Name}-{project.Version}.prj";
            await JSRuntime.InvokeAsync<object>("saveFile", fileName, json);
        }

        private async Task SaveProjectInstances(ProjectModel project)
        {
            string json = await SuperVRestClient.SaveProjectInstancesAsync(project.Id);
            string fileName = $"{project.Name}-{project.Version}.snp";
            await JSRuntime.InvokeAsync<object>("saveFile", fileName, json);
        }

        private async Task LoadProjectFromDefinition(IBrowserFile file)
        {
            using MemoryStream memoryStream = new();
            await file.OpenReadStream().CopyToAsync(memoryStream);
            await SuperVRestClient.LoadProjectFromDefinitionsAsync(memoryStream.ToArray());
            await projectsTable.ReloadServerData();
        }

        private async Task LoadProjectInstancesFromFile(IBrowserFile file, ProjectModel project)
        {
            if (ContextMenusRef.TryGetValue(project.Id, out var menu) && menu is not null)
            {
                await menu.CloseMenuAsync();
            }
            using MemoryStream memoryStream = new();
            await file.OpenReadStream().CopyToAsync(memoryStream);
            await SuperVRestClient.LoadProjectInstancesAsync(project.Id, memoryStream.ToArray());
        }
    }
}
