using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using pva.SuperV.Blazor.Components.Dialogs;
using pva.SuperV.Model;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Components.Pages;
public partial class Classes
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject]
    private IClassService ClassService { get; set; } = default!;
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    private IDialogService DialogService { get; set; } = default!;
    [Inject]
    private State State { get; set; } = default!;

    [Parameter]
    public string ProjectId { get; set; } = default!;

    private MudTable<ClassModel> itemsTable = default!;
    private string itemNameSearchString = default!;
    private int selectedRowNumber;

    private ClassModel? SelectedItem { get; set; } = default!;

    protected override void OnInitialized()
    {
        State.AddClassesBreadcrumb(ProjectId);
        base.OnInitialized();
    }

    private async Task<TableData<ClassModel>> ServerReload(TableState state, CancellationToken token)
    {
        ClassPagedSearchRequest request = new(state.Page + 1, state.PageSize, itemNameSearchString, null);
        PagedSearchResult<ClassModel> classes = await ClassService.SearchClassesAsync(ProjectId, request);
        TableData<ClassModel> itemsTableData = new() { TotalItems = classes.Count, Items = classes.Result };
        return itemsTableData;
    }

    private async Task CreateItem(MouseEventArgs e)
    {
        SelectedItem = null;
        State.EditedClass = null;
        NavigationManager.NavigateTo($"/class/{ProjectId}");
        await ReloadTable();
    }

    private async Task EditItem(MouseEventArgs e)
    {
        if (SelectedItem != null)
        {
            NavigationManager.NavigateTo($"/class/{ProjectId}/{SelectedItem.Name}");
            await ReloadTable();
        }
    }

    private void RowClickedEvent(TableRowClickEventArgs<ClassModel> _)
    {
        SelectedItem = itemsTable.SelectedItem;
        State.EditedClass = SelectedItem;
    }

    private string SelectedRowClassFunc(ClassModel item, int rowNumber)
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
        var parameters = new DialogParameters<DeleteConfirmationDialog> { { x => x.EntityDescription, $"class {itemId}" } };

        var dialog = await DialogService.ShowAsync<DeleteConfirmationDialog>($"Delete class", parameters);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            await ClassService.DeleteClassAsync(ProjectId, itemId);
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
