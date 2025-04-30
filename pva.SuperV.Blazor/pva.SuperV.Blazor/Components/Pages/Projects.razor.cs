using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using pva.SuperV.Blazor.Components.Dialogs;
using pva.SuperV.Model;
using pva.SuperV.Model.Projects;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class Projects
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        private IProjectService ProjectServiceClient { get; set; } = default!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        private IDialogService DialogService { get; set; } = default!;
        [Inject]
        private State State { get; set; } = default!;

        private MudTable<ProjectModel> itemsTable = default!;
        private string itemNameSearchString = default!;
        private int selectedRowNumber;

        private ProjectModel? SelectedItem { get; set; } = default!;

        protected override void OnInitialized()
        {
            State.SetProjectsBreadcrumb();
            base.OnInitialized();
        }

        private async Task<TableData<ProjectModel>> ServerReload(TableState state, CancellationToken _)
        {
            ProjectPagedSearchRequest request = new(state.Page + 1, state.PageSize, itemNameSearchString, null);
            PagedSearchResult<ProjectModel> projects = await ProjectServiceClient.SearchProjectsAsync(request);
            TableData<ProjectModel> itemsTableData = new() { TotalItems = projects.Count, Items = projects.Result };
            return itemsTableData;
        }

        private void CreateItem(MouseEventArgs e)
        {
            SelectedItem = null;
            State.CurrentProject = null;
            NavigationManager.NavigateTo("/project");
        }

        private void EditItem(MouseEventArgs e)
        {
            if (SelectedItem != null)
            {
                State.CurrentProject = SelectedItem;
                NavigationManager.NavigateTo($"/project/{SelectedItem.Id}");
            }
        }

        private void RowClickedEvent(TableRowClickEventArgs<ProjectModel> _)
        {
            SelectedItem = itemsTable.SelectedItem;
            State.CurrentProject = SelectedItem;
        }

        private string SelectedRowClassFunc(ProjectModel project, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                SelectedItem = null;
                return string.Empty;
            }
            else if (itemsTable.SelectedItem != null && itemsTable.SelectedItem.Equals(project))
            {
                selectedRowNumber = rowNumber;
                SelectedItem = itemsTable.SelectedItem;
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }

        private async Task Search(string args)
        {
            await ReloadTable();
        }

        private async Task CreateWipProjectFromRunnable(string runnableProjectId)
        {
            await ProjectServiceClient.CreateProjectFromRunnableAsync(runnableProjectId);
            await ReloadTable();
        }

        private async Task BuildWipProject(string wipProjectId)
        {
            await ProjectServiceClient.BuildProjectAsync(wipProjectId);
            await ReloadTable();
        }
        private async Task DeleteProject(string projectId)
        {
            var parameters = new DialogParameters<DeleteConfirmationDialog> { { x => x.EntityDescription, $"project {projectId}" } };

            var dialog = await DialogService.ShowAsync<DeleteConfirmationDialog>($"Delete project", parameters);
            var result = await dialog.Result;

            if (result is not null && !result.Canceled)
            {
                await ProjectServiceClient.UnloadProjectAsync(projectId);
                await ReloadTable();
            }
        }

        private async Task SaveProjectDefinitions(ProjectModel project)
        {
            StreamReader? streamReader = await ProjectServiceClient.GetProjectDefinitionsAsync(project.Id);
            if (streamReader != null)
            {
                string json = await streamReader.ReadToEndAsync();
                string fileName = $"{project.Name}-{project.Version}.prj";
                await JSRuntime.InvokeAsync<object>("saveFile", fileName, json);
            }
        }

        private async Task SaveProjectInstances(ProjectModel project)
        {
            StreamReader? streamReader = await ProjectServiceClient.GetProjectInstancesAsync(project.Id);
            if (streamReader != null)
            {
                string json = await streamReader.ReadToEndAsync();
                string fileName = $"{project.Name}-{project.Version}.snp";
                await JSRuntime.InvokeAsync<object>("saveFile", fileName, json);
            }
        }

        private async Task LoadProjectFromDefinition(IBrowserFile file)
        {
            using MemoryStream memoryStream = new();
            await file.OpenReadStream().CopyToAsync(memoryStream);
            byte[] fileContent = memoryStream.ToArray();
            using MemoryStream memoryStream2 = new(fileContent);
            _ = ProjectServiceClient.CreateProjectFromJsonDefinitionAsync(new StreamReader(memoryStream2));
            await ReloadTable();
        }

        private async Task LoadProjectInstancesFromFile(IBrowserFile file, ProjectModel project)
        {
            using MemoryStream memoryStream = new();
            await file.OpenReadStream().CopyToAsync(memoryStream);
            byte[] fileContent = memoryStream.ToArray();
            using MemoryStream memoryStream2 = new(fileContent);
            await ProjectServiceClient.LoadProjectInstancesAsync(project.Id, new StreamReader(memoryStream2));
        }

        private async Task ReloadTable()
        {
            selectedRowNumber = -1;
            SelectedItem = null;
            await itemsTable.ReloadServerData();
        }
    }
}
