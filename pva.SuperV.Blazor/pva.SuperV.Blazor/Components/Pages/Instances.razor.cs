using Microsoft.AspNetCore.Components;
using MudBlazor;
using pva.SuperV.Blazor.Components.Dialogs;
using pva.SuperV.Model;
using pva.SuperV.Model.Instances;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Components.Pages;
public partial class Instances
{
    [Inject]
    private IInstanceService InstanceService { get; set; } = default!;
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    private IDialogService DialogService { get; set; } = default!;
    [Inject]
    private State State { get; set; } = default!;

    [Parameter]
    public string ProjectId { get; set; } = default!;

    private string pageTitle = default!;
    private MudTable<InstanceModel> itemsTable = default!;
    private string itemNameSearchString = default!;
    private int selectedRowNumber;

    private InstanceModel? SelectedItem { get; set; }

    protected override void OnInitialized()
    {
        pageTitle = "Instances";
        State.SetInstancesBreadcrumb(ProjectId);
        base.OnInitialized();
    }

    private async Task<TableData<InstanceModel>> ServerReload(TableState state, CancellationToken _)
    {
        // TODO Add class name filtering
        InstancePagedSearchRequest request = new(state.Page + 1, state.PageSize, itemNameSearchString, null, null);
        PagedSearchResult<InstanceModel> projects = await InstanceService.SearchInstancesAsync(ProjectId, request);
        return new() { TotalItems = projects.Count, Items = projects.Result };
    }

    private void RowClickedEvent(TableRowClickEventArgs<InstanceModel> _)
    {
        SelectedItem = itemsTable.SelectedItem;
    }

    private string SelectedRowClassFunc(InstanceModel item, int rowNumber)
    {
        if (selectedRowNumber == rowNumber)
        {
            selectedRowNumber = -1;
            SelectedItem = null;
            return string.Empty;
        }
        else if (itemsTable.SelectedItem?.Equals(item) == true)
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

    private async Task Search(string _)
    {
        await ReloadTable();
    }

    private async Task DeleteItem(string itemId)
    {
        var parameters = new DialogParameters<DeleteConfirmationDialog> { { x => x.EntityDescription, $"instance {itemId}" } };

        var dialog = await DialogService.ShowAsync<DeleteConfirmationDialog>("Delete instance", parameters);
        var result = await dialog.Result;

        if (result?.Canceled == false)
        {
            await InstanceService.DeleteInstanceAsync(ProjectId, itemId);
            await ReloadTable();
        }
    }

    private async Task ReloadTable()
    {
        selectedRowNumber = -1;
        SelectedItem = null;
        await itemsTable.ReloadServerData();
    }
}
