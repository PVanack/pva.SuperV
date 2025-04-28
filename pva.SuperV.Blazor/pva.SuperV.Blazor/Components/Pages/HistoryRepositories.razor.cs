using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using pva.SuperV.Blazor.Components.Dialogs;
using pva.SuperV.Model.HistoryRepositories;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Components.Pages;
public partial class HistoryRepositories
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject]
    private IHistoryRepositoryService HistoryRepositoryService { get; set; } = default!;
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    private IDialogService DialogService { get; set; } = default!;
    [Inject]
    private State State { get; set; } = default!;

    [Parameter]
    public string ProjectId { get; set; } = default!;

    private MudTable<HistoryRepositoryModel> itemsTable = default!;
    private int selectedRowNumber;

    private HistoryRepositoryModel? SelectedItem { get; set; } = default!;

    protected override void OnInitialized()
    {
        State.AddHistoryRepositoriesBreadcrumb(ProjectId);
        base.OnInitialized();
    }

    private async Task<TableData<HistoryRepositoryModel>> ServerReload(TableState state, CancellationToken token)
    {
        List<HistoryRepositoryModel> historyRepositories = await HistoryRepositoryService.GetHistoryRepositoriesAsync(ProjectId);
        TableData<HistoryRepositoryModel> itemsTableData = new() { TotalItems = historyRepositories.Count, Items = historyRepositories };
        return itemsTableData;
    }

    private async Task CreateItem(MouseEventArgs e)
    {
        SelectedItem = null;
        State.EditedFieldFormatter = null;
        NavigationManager.NavigateTo($"/history-respository/{ProjectId}");
        await ReloadTable();
    }

    private async Task EditItem(MouseEventArgs e)
    {
        if (SelectedItem != null)
        {
            NavigationManager.NavigateTo($"/history-respository/{ProjectId}/{SelectedItem.Name}");
            await ReloadTable();
        }
    }

    private void RowClickedEvent(TableRowClickEventArgs<HistoryRepositoryModel> _)
    {
        SelectedItem = itemsTable.SelectedItem;
        State.EditedHistoryRepository = SelectedItem;
    }

    private string SelectedRowClassFunc(HistoryRepositoryModel item, int rowNumber)
    {
        if (selectedRowNumber == rowNumber)
        {
            selectedRowNumber = -1;
            SelectedItem = null;
            return string.Empty;
        }
        else if (itemsTable.SelectedItem != null && itemsTable.SelectedItem.Equals(item))
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

    private async Task DeleteItem(string itemId)
    {
        var parameters = new DialogParameters<DeleteConfirmationDialog> { { x => x.EntityDescription, $"history repository{itemId}" } };

        var dialog = await DialogService.ShowAsync<DeleteConfirmationDialog>($"Delete history repository", parameters);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            await HistoryRepositoryService.DeleteHistoryRepositoryAsync(ProjectId, itemId);
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