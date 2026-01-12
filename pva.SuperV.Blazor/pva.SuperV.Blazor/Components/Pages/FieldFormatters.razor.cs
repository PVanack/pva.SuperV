using Microsoft.AspNetCore.Components;
using MudBlazor;
using pva.SuperV.Blazor.Components.Dialogs;
using pva.SuperV.Model;
using pva.SuperV.Model.FieldFormatters;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Components.Pages;

public partial class FieldFormatters
{
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

    private string pageTitle = default!;
    private MudTable<FieldFormatterModel> itemsTable = default!;
    private string itemNameSearchString = default!;
    private int selectedRowNumber;

    private FieldFormatterModel? SelectedItem { get; set; }

    protected override void OnInitialized()
    {
        pageTitle = "Field formatters";
        State.SetFieldFormattersBreadcrumb(ProjectId);
        base.OnInitialized();
    }

    private async Task<TableData<FieldFormatterModel>> ServerReload(TableState state, CancellationToken _)
    {
        FieldFormatterPagedSearchRequest request = new(state.Page + 1, state.PageSize, itemNameSearchString, null);
        PagedSearchResult<FieldFormatterModel> projects = await FieldFormatterService.SearchFieldFormattersAsync(ProjectId, request);
        return new() { TotalItems = projects.Count, Items = projects.Result };
    }

    private void RowClickedEvent(TableRowClickEventArgs<FieldFormatterModel> _)
    {
        SelectedItem = itemsTable.SelectedItem;
    }

    private string SelectedRowClassFunc(FieldFormatterModel item, int rowNumber)
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
        var parameters = new DialogParameters<DeleteConfirmationDialog> { { x => x.EntityDescription, $"field formatter {itemId}" } };

        var dialog = await DialogService.ShowAsync<DeleteConfirmationDialog>("Delete field formatter", parameters);
        var result = await dialog.Result;

        if (result?.Canceled == false)
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
