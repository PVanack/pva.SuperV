using Microsoft.AspNetCore.Components;
using MudBlazor;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.HistoryRepositories;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class HistorizationProcessing
    {
        [Inject]
        State State { get; set; } = default!;
        [Inject]
        IHistoryRepositoryService HistoryRepositoryService { get; set; } = default!;

        [CascadingParameter(Name = "EditedFieldProcessing")]
        EditedFieldProcessing EditedFieldProcessing { get; set; } = default!;
        [CascadingParameter(Name = "ClassAvailableFields")]
        private List<FieldDefinitionModel> ClassAvailableFields { get; set; } = [];
        [CascadingParameter(Name = "TriggeringFieldName")]
        private string TriggeringFieldName { get; set; } = default!;

        private EditedHistorizationProcessing? editedHistorizationFieldProcessing = default!;
        private List<string> HistoryRepositoryNames { get; set; } = [];
        private Dictionary<int, MudSelect<string>?> FieldsToHistorizeSelects { get; set; } = [];

        protected override async Task OnParametersSetAsync()
        {
            editedHistorizationFieldProcessing = EditedFieldProcessing as EditedHistorizationProcessing;
            if (editedHistorizationFieldProcessing != null)
            {
                HistoryRepositoryNames = await GetHistoryRepositories();
                FieldsToHistorizeSelects.Clear();
                for (int index = 0; index < editedHistorizationFieldProcessing!.FieldsToHistorize.Count; index++)
                {
                    FieldsToHistorizeSelects.Add(index, null);
                }
            }
            await base.OnParametersSetAsync();
        }

        private List<string> GetFieldNamesForType(FieldDefinitionModel? fieldToBeSearched)
        {
            List<string> allFieldNames = [""];
            allFieldNames.AddRange([.. ClassAvailableFields
                .Where(field => (fieldToBeSearched == null || field.GetType().Equals(fieldToBeSearched.GetType()) &&
                        field.Name != TriggeringFieldName))
                .Select(field => field.Name)]);
            return allFieldNames;
        }
        private List<string> GetFieldNames()
        {
            return GetFieldNamesForType(null);
        }

        private async Task<List<string>> GetHistoryRepositories()
        {
            List<HistoryRepositoryModel> repositories = await HistoryRepositoryService.GetHistoryRepositoriesAsync(State.CurrentProject!.Id);
            return [.. repositories.Select(historyRepository => historyRepository.Name)];
        }

        private void SetFieldToHistorize(int index)
        {
            editedHistorizationFieldProcessing!.FieldsToHistorize[index] = FieldsToHistorizeSelects[index]!.Value!;
        }

        private void RemoveFieldToHistorize(int index)
        {
            editedHistorizationFieldProcessing!.FieldsToHistorize.RemoveAt(index);
        }
    }
}