using Microsoft.AspNetCore.Components;
using MudBlazor;
using pva.SuperV.Blazor.Components.Dialogs;
using pva.SuperV.Model;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Components.Pages;

public partial class ScriptDefinitions
{
    [Inject]
    private IScriptService ScriptService { get; set; } = default!;
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    private IDialogService DialogService { get; set; } = default!;
    [Inject]
    private State State { get; set; } = default!;

    [Parameter]
    public string ProjectId { get; set; } = default!;

    private MudTable<ScriptDefinitionModel> itemsTable = default!;
    private string itemNameSearchString = default!;
    private int selectedRowNumber;

    private ScriptDefinitionModel? SelectedItem { get; set; }

    protected override void OnInitialized()
    {
        State.SetClassesBreadcrumb(ProjectId);
        base.OnInitialized();
    }

    private async Task<TableData<ScriptDefinitionModel>> ServerReload(TableState state, CancellationToken _)
    {
        List<ScriptDefinitionModel> scripts = await ScriptService.GetScriptsAsync(ProjectId);
        return new() { TotalItems = scripts.Count, Items = scripts };
    }

    private void RowClickedEvent(TableRowClickEventArgs<ScriptDefinitionModel> _)
    {
        SelectedItem = itemsTable.SelectedItem;
    }

    private string SelectedRowScriptFunc(ScriptDefinitionModel item, int rowNumber)
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
        var parameters = new DialogParameters<DeleteConfirmationDialog> { { x => x.EntityDescription, $"script {itemId}" } };

        var dialog = await DialogService.ShowAsync<DeleteConfirmationDialog>("Delete script", parameters);
        var result = await dialog.Result;

        if (result?.Canceled == false)
        {
            await ScriptService.DeleteScriptAsync(ProjectId, itemId);
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
