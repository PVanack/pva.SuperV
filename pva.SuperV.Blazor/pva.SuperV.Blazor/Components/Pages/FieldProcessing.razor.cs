using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;
using System.ComponentModel.DataAnnotations;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class FieldProcessing
    {
        public const string AlarmStateProcessingType = "Alarm state";
        public const string HistorizationProcessingType = "Historization";

        [Inject]
        private IFieldProcessingService FieldProcessingService { get; set; } = default!;
        [Inject]
        IFieldDefinitionService FieldDefinitionService { get; set; } = default!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        private State State { get; set; } = default!;

        [Parameter]
        public string ProjectId { get; set; } = default!;
        [Parameter]
        public string ClassName { get; set; } = default!;
        [Parameter]
        public string FieldName { get; set; } = default!;
        [Parameter]
        public string ProcessingName { get; set; } = default!;

        private static readonly Dictionary<string, Func<EditedFieldProcessing, EditedFieldProcessing>> fieldProcessingToSubtype = new()
        {
            { AlarmStateProcessingType , (editedFieldProcessing) => new EditedAlarmStateProcessing(editedFieldProcessing.Name, editedFieldProcessing.TrigerringFieldName) },
            { HistorizationProcessingType , (editedFieldProcessing) => new EditedHistorizationProcessing(editedFieldProcessing.Name, editedFieldProcessing.TrigerringFieldName) }
        };

        private static readonly Dictionary<Type, Func<EditedFieldProcessing, string, FieldValueProcessingModel>> fieldProcessingToModel = new()
        {
            {typeof(EditedAlarmStateProcessing) , (editedFieldProcessing, triggeringFieldName) => CreateAlarmStateProcessingModel(editedFieldProcessing, triggeringFieldName)},
            {typeof(EditedHistorizationProcessing) , (editedFieldProcessing, triggeringFieldName) => CreateHistorizationProcessingModel(editedFieldProcessing, triggeringFieldName)}
        };

        private static readonly Dictionary<Type, Func<FieldValueProcessingModel, EditedFieldProcessing>> fieldProcessingFromModel = new()
        {
            {typeof(AlarmStateProcessingModel) , (fieldProcessing) => CreateEditableAlarmStateProcessing(fieldProcessing)},
            {typeof(HistorizationProcessingModel) , (fieldProcessing) => CreateEditableHistorizationProcessing(fieldProcessing)}
        };

        private static AlarmStateProcessingModel CreateAlarmStateProcessingModel(EditedFieldProcessing editedFieldProcessing, string triggeringFieldName)
        {
            EditedAlarmStateProcessing? alarmStateProcessing = editedFieldProcessing! as EditedAlarmStateProcessing;
            return new AlarmStateProcessingModel(editedFieldProcessing!.Name, triggeringFieldName,
                alarmStateProcessing!.HighHighLimitFieldName, alarmStateProcessing!.HighLimitFieldName, alarmStateProcessing!.LowLimitFieldName, alarmStateProcessing!.LowLowLimitFieldName,
                alarmStateProcessing!.DeadbandFieldName, alarmStateProcessing!.AlarmStateFieldName, alarmStateProcessing!.AckStateFieldName);
        }

        private static HistorizationProcessingModel CreateHistorizationProcessingModel(EditedFieldProcessing editedFieldProcessing, string triggeringFieldName)
        {
            EditedHistorizationProcessing? historizationProcessing = editedFieldProcessing! as EditedHistorizationProcessing;
            return new HistorizationProcessingModel(editedFieldProcessing!.Name, triggeringFieldName,
                historizationProcessing!.HistoryRepositoryName, historizationProcessing.TimestampFieldName,
                [.. historizationProcessing.FieldsToHistorize.Select(f => f.Name)]);
        }

        private static EditedAlarmStateProcessing CreateEditableAlarmStateProcessing(FieldValueProcessingModel fieldProcessingModel)
        {
            AlarmStateProcessingModel? alarmStateProcessing = fieldProcessingModel as AlarmStateProcessingModel;
            return new EditedAlarmStateProcessing(alarmStateProcessing!);
        }

        private static EditedHistorizationProcessing CreateEditableHistorizationProcessing(FieldValueProcessingModel fieldProcessingModel)
        {
            HistorizationProcessingModel? historizationProcessing = fieldProcessingModel as HistorizationProcessingModel;
            return new EditedHistorizationProcessing(historizationProcessing!);
        }

        private string pageTitle = default!;
        private bool success;
        private bool isModification;

        private EditedFieldProcessing EditedFieldProcessing { get; set; } = default!;
        private List<FieldDefinitionModel> ClassAvailableFields { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            isModification = !String.IsNullOrEmpty(ProcessingName);
            ClassAvailableFields = await FieldDefinitionService.GetFieldsAsync(ProjectId, ClassName);
            EditedFieldProcessing = new();
            if (isModification)
            {
                FieldValueProcessingModel fieldProcessing = await FieldProcessingService.GetFieldProcessingAsync(ProjectId, ClassName, FieldName, ProcessingName);
                if (fieldProcessingFromModel.TryGetValue(fieldProcessing!.GetType(), out var createFunc))
                {
                    EditedFieldProcessing = createFunc(fieldProcessing);
                    State.SetFieldProcessingBreadcrumb(ProjectId, ClassName, FieldName, EditedFieldProcessing.Name);
                }
            }
            pageTitle = isModification ? $"{EditedFieldProcessing.ProcessingType} {EditedFieldProcessing!.Name}" : "New field processing";
            await base.OnInitializedAsync();
        }

        private async Task OnValidSubmit(EditContext _)
        {
            success = true;
            FieldValueProcessingModel fieldProcessing = MapFieldProcessing(EditedFieldProcessing, FieldName);
            if (isModification)
            {
                await FieldProcessingService.UpdateFieldProcessingAsync(ProjectId, ClassName, FieldName, EditedFieldProcessing.Name, fieldProcessing);
            }
            else
            {
                await FieldProcessingService.CreateFieldProcessingAsync(ProjectId, ClassName, FieldName, fieldProcessing);
            }
            GoBackToFieldProcessings();
        }

        private void CancelSubmit()
        {
            GoBackToFieldProcessings();
        }

        private void GoBackToFieldProcessings()
        {
            NavigationManager.NavigateTo($"/field-processings/{ProjectId}/{ClassName}/{FieldName}");
        }

        private static List<string> GetFieldProcessingTypes()
        {
            return [.. fieldProcessingToSubtype.Keys];
        }

        private void OnFormatterTypeChanged(string selectedType)
        {
            if (!String.IsNullOrEmpty(selectedType) && fieldProcessingToSubtype.TryGetValue(selectedType, out var createFunc))
            {
                EditedFieldProcessing.ProcessingType = selectedType;
                EditedFieldProcessing = createFunc(EditedFieldProcessing);
            }
            StateHasChanged();
        }

        private static FieldValueProcessingModel MapFieldProcessing(EditedFieldProcessing editedFieldProcessing, string triggeringFieldName)
        {
            if (fieldProcessingToModel.TryGetValue(editedFieldProcessing.GetType(), out var mapFunc))
            {
                return mapFunc(editedFieldProcessing, triggeringFieldName);
            }
            throw new InvalidOperationException($"No mapping for field value processing type {editedFieldProcessing.GetType()}");
        }

        public static string GetProcessingType(FieldValueProcessingModel fieldProcessing)
        {
            if (fieldProcessing is null)
                return String.Empty;
            return fieldProcessing switch
            {
                AlarmStateProcessingModel => AlarmStateProcessingType,
                HistorizationProcessingModel => HistorizationProcessingType,
                _ => "Unknown"
            };

        }
    }

    public class EditedFieldProcessing
    {
        public EditedFieldProcessing() : this("", "", "") { }
        protected EditedFieldProcessing(string name, string processingType, string trigerringFieldName)
        {
            Name = name;
            ProcessingType = processingType;
            TrigerringFieldName = trigerringFieldName;
        }

        protected EditedFieldProcessing(FieldValueProcessingModel fieldProcessing)
        {
            this.Name = fieldProcessing.Name;
            this.ProcessingType = FieldProcessing.GetProcessingType(fieldProcessing);
            TrigerringFieldName = fieldProcessing.TrigerringFieldName;
        }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        public string ProcessingType { get; set; }
        public string TrigerringFieldName { get; set; }
    }

    public class EditedAlarmStateProcessing : EditedFieldProcessing
    {
        public EditedAlarmStateProcessing(string name, string trigerringFieldName) : base(name, FieldProcessing.AlarmStateProcessingType, trigerringFieldName)
        {
        }

        public EditedAlarmStateProcessing(AlarmStateProcessingModel alarmStateProcessing) : base(alarmStateProcessing)
        {
            HighHighLimitFieldName = alarmStateProcessing.HighHighLimitFieldName;
            HighLimitFieldName = alarmStateProcessing.HighLimitFieldName;
            LowLimitFieldName = alarmStateProcessing.LowLimitFieldName;
            LowLowLimitFieldName = alarmStateProcessing.LowLowLimitFieldName;
            DeadbandFieldName = alarmStateProcessing.DeadbandFieldName;
            AlarmStateFieldName = alarmStateProcessing.AlarmStateFieldName;
            AckStateFieldName = alarmStateProcessing.AckStateFieldName;
        }

        public string? HighHighLimitFieldName { get; set; } = null;
        public string HighLimitFieldName { get; set; } = string.Empty;
        public string LowLimitFieldName { get; set; } = string.Empty;
        public string? LowLowLimitFieldName { get; set; } = null;
        public string? DeadbandFieldName { get; set; } = null;
        public string AlarmStateFieldName { get; set; } = string.Empty;
        public string? AckStateFieldName { get; set; } = null;
    }

    public class HistorizedField(string fieldName)
    {
        public string Name { get; set; } = fieldName;
    }
    public class EditedHistorizationProcessing : EditedFieldProcessing
    {
        public EditedHistorizationProcessing(string name, string trigerringFieldName) : base(name, FieldProcessing.HistorizationProcessingType, trigerringFieldName)
        {
        }

        public EditedHistorizationProcessing(HistorizationProcessingModel historizationProcessing) : base(historizationProcessing)
        {
            HistoryRepositoryName = historizationProcessing.HistoryRepositoryName;
            TimestampFieldName = historizationProcessing.TimestampFieldName;
            FieldsToHistorize = [.. historizationProcessing.FieldsToHistorize.Select(fieldName => new HistorizedField(fieldName))];
        }

        public string HistoryRepositoryName { get; set; } = "";
        public string? TimestampFieldName { get; set; } = null;
        public List<HistorizedField> FieldsToHistorize { get; set; } = [];
    }
}
