using Microsoft.AspNetCore.Components;
using MudBlazor;
using pva.SuperV.Blazor.Components.Dialogs;
using pva.SuperV.Model.HistoryRepositories;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Components.Pages;
public partial class HistoryRepositories
{
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

    private HistoryRepositoryModel? SelectedItem { get; set; }

    protected override void OnInitialized()
    {
        State.SetHistoryRepositoriesBreadcrumb(ProjectId);
        base.OnInitialized();
    }

    private async Task<TableData<HistoryRepositoryModel>> ServerReload(TableState _, CancellationToken __)
    {
        List<HistoryRepositoryModel> historyRepositories = await HistoryRepositoryService.GetHistoryRepositoriesAsync(ProjectId);
        return new() { TotalItems = historyRepositories.Count, Items = historyRepositories };
    }

    private void RowClickedEvent(TableRowClickEventArgs<HistoryRepositoryModel> _)
    {
        SelectedItem = itemsTable.SelectedItem;
    }

    private string SelectedRowClassFunc(HistoryRepositoryModel item, int rowNumber)
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

    private async Task DeleteItem(string itemId)
    {
        var parameters = new DialogParameters<DeleteConfirmationDialog> { { x => x.EntityDescription, $"history repository {itemId}" } };

        var dialog = await DialogService.ShowAsync<DeleteConfirmationDialog>("Delete history repository", parameters);
        var result = await dialog.Result;

        if (result?.Canceled == false)
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