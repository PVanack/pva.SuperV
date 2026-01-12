using Microsoft.AspNetCore.Components;
using MudBlazor;
using pva.SuperV.Blazor.Components.Dialogs;
using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Components.Pages;

public partial class FieldProcessings
{
    [Inject]
    private IFieldProcessingService FieldProcessingService { get; set; } = default!;
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    private IDialogService DialogService { get; set; } = default!;
    [Inject]
    private State State { get; set; } = default!;

    [Parameter]
    public string ProjectId { get; set; } = default!;
    [Parameter]
    public string ClassName { get; set; } = default!;
    [Parameter]
    public string FieldName { get; set; } = default!;

    private string pageTitle = default!;
    private MudTable<FieldValueProcessingModel> itemsTable = default!;
    private string itemNameSearchString = default!;
    private int selectedRowNumber;

    private FieldValueProcessingModel? SelectedItem { get; set; }

    protected override void OnInitialized()
    {
        pageTitle = "Field value processings";
        State.SetFieldProcessingsBreadcrumb(ProjectId, ClassName, FieldName);
        base.OnInitialized();
    }

    private async Task<TableData<FieldValueProcessingModel>> ServerReload(TableState _, CancellationToken __)
    {
        List<FieldValueProcessingModel> fieldProcessings = await FieldProcessingService.GetFieldProcessingsAsync(ProjectId, ClassName, FieldName);
        return new() { TotalItems = fieldProcessings.Count, Items = fieldProcessings };
    }

    private void RowClickedEvent(TableRowClickEventArgs<FieldValueProcessingModel> _)
    {
        SelectedItem = itemsTable.SelectedItem;
    }

    private string SelectedRowClassFunc(FieldValueProcessingModel item, int rowNumber)
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
        var parameters = new DialogParameters<DeleteConfirmationDialog> { { x => x.EntityDescription, $"field value processing {itemId}" } };

        var dialog = await DialogService.ShowAsync<DeleteConfirmationDialog>("Delete field value processing", parameters);
        var result = await dialog.Result;

        if (result?.Canceled == false)
        {
            await FieldProcessingService.DeleteFieldProcessingAsync(ProjectId, ClassName, FieldName, itemId);
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
