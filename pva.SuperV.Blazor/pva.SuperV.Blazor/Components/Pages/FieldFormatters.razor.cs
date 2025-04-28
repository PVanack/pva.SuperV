using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using pva.SuperV.Blazor.Components.Dialogs;
using pva.SuperV.Model;
using pva.SuperV.Model.FieldFormatters;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Components.Pages;
public partial class FieldFormatters
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject]
    private IFieldFormatterService FieldFormatterService { get; set; } = default!;
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    private IDialogService DialogService { get; set; } = default!;
    [Inject]
    private State State { get; set; } = default!;

    [Parameter]
    public string ProjectId { get; set; } = default!;

    private MudTable<FieldFormatterModel> itemsTable = default!;
    private string itemNameSearchString = default!;
    private int selectedRowNumber;

    private FieldFormatterModel? SelectedItem { get; set; } = default!;

    protected override void OnInitialized()
    {
        State.AddFieldFormattersBreadcrumb(ProjectId);
        base.OnInitialized();
    }

    private async Task<TableData<FieldFormatterModel>> ServerReload(TableState state, CancellationToken token)
    {
        FieldFormatterPagedSearchRequest request = new(state.Page + 1, state.PageSize, itemNameSearchString, null);
        PagedSearchResult<FieldFormatterModel> projects = await FieldFormatterService.SearchFieldFormattersAsync(ProjectId, request);
        TableData<FieldFormatterModel> itemsTableData = new() { TotalItems = projects.Count, Items = projects.Result };
        return itemsTableData;
    }

    private async Task CreateItem(MouseEventArgs e)
    {
        SelectedItem = null;
        State.EditedFieldFormatter = null;
        NavigationManager.NavigateTo($"/field-formatter/{ProjectId}");
        await ReloadTable();
    }

    private async Task EditItem(MouseEventArgs e)
    {
        if (SelectedItem != null)
        {
            NavigationManager.NavigateTo($"/field-formatter/{ProjectId}/{SelectedItem.Name}");
            await ReloadTable();
        }
    }

    private void RowClickedEvent(TableRowClickEventArgs<FieldFormatterModel> _)
    {
        SelectedItem = itemsTable.SelectedItem;
        State.EditedFieldFormatter = SelectedItem;
    }

    private string SelectedRowClassFunc(FieldFormatterModel item, int rowNumber)
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

    private async Task Search(string args)
    {
        await ReloadTable();
    }

    private async Task DeleteItem(string itemId)
    {
        var parameters = new DialogParameters<DeleteConfirmationDialog> { { x => x.EntityDescription, $"field formatter {itemId}" } };

        var dialog = await DialogService.ShowAsync<DeleteConfirmationDialog>($"Delete field formatter", parameters);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            await FieldFormatterService.DeleteFieldFormatterAsync(ProjectId, itemId);
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
