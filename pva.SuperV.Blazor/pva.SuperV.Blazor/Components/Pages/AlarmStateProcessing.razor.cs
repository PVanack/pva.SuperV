using Microsoft.AspNetCore.Components;
using MudBlazor;
using pva.SuperV.Model.FieldDefinitions;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class AlarmStateProcessing
    {
        [CascadingParameter(Name = "EditedFieldProcessing")]
        EditedFieldProcessing EditedFieldProcessing { get; set; } = default!;
        [CascadingParameter(Name = "ClassAvailableFields")]
        private List<FieldDefinitionModel> ClassAvailableFields { get; set; } = [];
        [CascadingParameter(Name = "TriggeringFieldName")]
        private string TriggeringFieldName { get; set; } = default!;

        private EditedAlarmStateProcessing? editedAlarmStateFieldProcessing;
        FieldDefinitionModel triggeringField = default!;

        protected override Task OnParametersSetAsync()
        {
            editedAlarmStateFieldProcessing = EditedFieldProcessing as EditedAlarmStateProcessing;
            triggeringField = ClassAvailableFields.First(field => field.Name.Equals(TriggeringFieldName));
            return base.OnParametersSetAsync();
        }

        private List<string> GetFieldNamesForType(FieldDefinitionModel? fieldToBeSearched)
        {
            List<string> allFieldNames = [""];
            allFieldNames.AddRange([.. ClassAvailableFields
                .Where(field => fieldToBeSearched == null ||
                (field.GetType().Equals(fieldToBeSearched.GetType()) &&
                  field.Name != TriggeringFieldName))
                .Select(field => field.Name)]);
            return allFieldNames;
        }
        private List<string> GetFieldNames()
        {
            return GetFieldNamesForType(null);
        }
    }
}