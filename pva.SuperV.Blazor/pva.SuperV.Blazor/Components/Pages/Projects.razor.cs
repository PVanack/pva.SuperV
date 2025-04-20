using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using pva.SuperV.Blazor.Components.Dialogs;
using pva.SuperV.Blazor.SuperVClient;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class Projects
    {
        private readonly HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;
        private readonly bool hidePageNumber = false;
        private readonly bool hidePagination = false;
        private readonly bool hideRowsPerPage = false;
        private readonly string rowsPerPageString = "Rows per page:";
        private readonly string infoFormat = "{first_item}-{last_item} of {all_items}";
        private readonly string allItemsText = "All";

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

        protected override void OnInitialized()
        {
            State.SetProjectsBreadcrumb();
            base.OnInitialized();
        }
        private async Task<TableData<ProjectModel>> ServerReload(TableState state, CancellationToken token)
        {
            ProjectPagedSearchRequest request = new() { PageNumber = state.Page + 1, PageSize = state.PageSize, NameFilter = projectNameSearchString };
            PagedSearchResultOfProjectModel projects = await SuperVRestClient.SearchProjectsAsync(request, token);
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
    }
}
