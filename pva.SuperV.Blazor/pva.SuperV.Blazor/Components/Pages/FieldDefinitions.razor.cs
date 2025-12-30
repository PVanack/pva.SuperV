using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using pva.SuperV.Blazor.Components.Dialogs;
using pva.SuperV.Model;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class FieldDefinitions
    {
        [Inject]
        private IFieldDefinitionService FieldDefinitionService { get; set; } = default!;
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

        private string pageTitle = default!;
        private MudTable<FieldDefinitionModel> itemsTable = default!;
        private string itemNameSearchString = default!;
        private int selectedRowNumber;

        private FieldDefinitionModel? SelectedItem { get; set; }

        protected override void OnInitialized()
        {
            pageTitle = "Fields";
            State.SetFieldDefinitionsBreadcrumb(ProjectId, ClassName);
            base.OnInitialized();
        }

        private async Task<TableData<FieldDefinitionModel>> ServerReload(TableState state, CancellationToken _)
        {
            FieldDefinitionPagedSearchRequest request = new(state.Page + 1, state.PageSize, itemNameSearchString, null);
            PagedSearchResult<FieldDefinitionModel> projects = await FieldDefinitionService.SearchFieldsAsync(ProjectId, ClassName, request);
            return new() { TotalItems = projects.Count, Items = projects.Result };
        }

        private async Task CreateItem(MouseEventArgs _)
        {
            SelectedItem = null;
            NavigationManager.NavigateTo($"/field-definition/{ProjectId}/{ClassName}");
            await ReloadTable();
        }

        private void RowClickedEvent(TableRowClickEventArgs<FieldDefinitionModel> _)
        {
            SelectedItem = itemsTable.SelectedItem;
        }

        private string SelectedRowClassFunc(FieldDefinitionModel item, int rowNumber)
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
            var parameters = new DialogParameters<DeleteConfirmationDialog> { { x => x.EntityDescription, $"field definition {itemId}" } };

            var dialog = await DialogService.ShowAsync<DeleteConfirmationDialog>("Delete field definition", parameters);
            var result = await dialog.Result;

            if (result?.Canceled == false)
            {
                await FieldDefinitionService.DeleteFieldAsync(ProjectId, ClassName, itemId);
                await ReloadTable();
            }
        }

        private async Task ReloadTable()
        {
            selectedRowNumber = -1;
            SelectedItem = null;
            await itemsTable.ReloadServerData();
        }

        private static string GetFieldType(FieldDefinitionModel field)
            => field switch
            {
                BoolFieldDefinitionModel => FieldDefinition.BooleanType,
                DateTimeFieldDefinitionModel => FieldDefinition.DateTimeType,
                DoubleFieldDefinitionModel => FieldDefinition.DoubleType,
                FloatFieldDefinitionModel => FieldDefinition.FloatType,
                IntFieldDefinitionModel => FieldDefinition.IntType,
                LongFieldDefinitionModel => FieldDefinition.LongType,
                ShortFieldDefinitionModel => FieldDefinition.ShortType,
                StringFieldDefinitionModel => FieldDefinition.StringType,
                TimeSpanFieldDefinitionModel => FieldDefinition.TimeSpanType,
                UintFieldDefinitionModel => FieldDefinition.UintType,
                UlongFieldDefinitionModel => FieldDefinition.UlongType,
                UshortFieldDefinitionModel => FieldDefinition.UshortType,
                _ => throw new NotImplementedException($"{field.GetType()} unhmadled")
            };
    }
}
