using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using pva.Helpers.Extensions;
using pva.SuperV.Model.FieldFormatters;
using pva.SuperV.Model.Services;
using System.ComponentModel.DataAnnotations;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class FieldFormatter
    {
        public const string EnumFormatterType = "Enum";

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

        [Parameter]
        public string FieldFormatterName { get; set; } = default!;

        private static readonly Dictionary<string, Func<EditedFieldFormatter, EditedFieldFormatter>> fieldFormatterToSubtype = new()
        {
            { EnumFormatterType , (editedFieldFormatter) => new EditedEnumFieldFormatter(editedFieldFormatter.Name, editedFieldFormatter.FormatterType, []) }
        };

        private static readonly Dictionary<Type, Func<EditedFieldFormatter, FieldFormatterModel>> fieldFormatterToModel = new()
        {
            {typeof(EditedEnumFieldFormatter) , (editedFieldFormatter) => CreateEnumFormatterModel(editedFieldFormatter)}
        };

        private static readonly Dictionary<Type, Func<FieldFormatterModel, EditedFieldFormatter>> fieldFormatterFromModel = new()
        {
            {typeof(EnumFormatterModel) , (fieldFormatter) => CreateEditableEnumFormatter(fieldFormatter)}
        };

        private static EnumFormatterModel CreateEnumFormatterModel(EditedFieldFormatter editedFieldFormatter)
        {
            EditedEnumFieldFormatter? enumFormatter = editedFieldFormatter! as EditedEnumFieldFormatter;
            return new EnumFormatterModel(editedFieldFormatter!.Name, enumFormatter!.GetEnumValues());
        }

        private static EditedEnumFieldFormatter CreateEditableEnumFormatter(FieldFormatterModel fieldFormatter)
        {
            EnumFormatterModel? enumFormatter = fieldFormatter as EnumFormatterModel;
            return new EditedEnumFieldFormatter(enumFormatter!);
        }

        private string pageTitle = default!;
        private bool success;
        private bool isModification;

        private EditedFieldFormatter EditedFieldFormatter { get; set; } = default!;

        protected override Task OnInitializedAsync()
        {
            isModification = !String.IsNullOrEmpty(FieldFormatterName) && State.EditedFieldFormatter != null;
            pageTitle = isModification ? $"{GetFormatterType(State.EditedFieldFormatter)} {State.EditedFieldFormatter!.Name}" : "New field formatter";
            EditedFieldFormatter = new();
            if (isModification && fieldFormatterFromModel.TryGetValue(State.EditedFieldFormatter!.GetType(), out var createFunc))
            {
                EditedFieldFormatter = createFunc(State.EditedFieldFormatter);
            }
            State.AddFieldFormatterBreadcrumb(ProjectId, State.EditedFieldFormatter);
            return base.OnInitializedAsync();
        }

        private async Task OnValidSubmit(EditContext context)
        {
            success = true;
            FieldFormatterModel fieldFormatter = MapFieldFormatter(EditedFieldFormatter);
            if (isModification)
            {
                await FieldFormatterService.UpdateFieldFormatterAsync(ProjectId, EditedFieldFormatter.Name, fieldFormatter);
            }
            else
            {
                CreateFieldFormatterRequest createRequest = new(fieldFormatter);
                await FieldFormatterService.CreateFieldFormatterAsync(ProjectId, createRequest);
            }
            GoBackToFieldFormatters();
        }

        private void CancelSubmit()
        {
            GoBackToFieldFormatters();
        }

        private void GoBackToFieldFormatters()
        {
            State.EditedFieldFormatter = null;
            State.RemoveLastBreadcrumb();
            NavigationManager.NavigateTo($"/field-formatters/{ProjectId}");
        }

        private static List<string> GetFieldFormatterTypes()
        {
            return [.. fieldFormatterToSubtype.Keys];
        }

        private void OnFormatterTypeChanged(string selectedType)
        {
            if (!String.IsNullOrEmpty(selectedType) && fieldFormatterToSubtype.TryGetValue(selectedType, out var createFunc))
            {
                EditedFieldFormatter.FormatterType = selectedType;
                EditedFieldFormatter = createFunc(EditedFieldFormatter);
            }
            StateHasChanged();
        }
        private static FieldFormatterModel MapFieldFormatter(EditedFieldFormatter editedFieldFormatter)
        {
            if (fieldFormatterToModel.TryGetValue(editedFieldFormatter.GetType(), out var mapFunc))
            {
                return mapFunc(editedFieldFormatter);
            }
            throw new InvalidOperationException($"No mapping for field formatter type {editedFieldFormatter.GetType()}");
        }

        public static string GetFormatterType(FieldFormatterModel? fieldFormatter)
        {
            if (fieldFormatter is null)
                return String.Empty;
            return fieldFormatter switch
            {
                EnumFormatterModel => FieldFormatter.EnumFormatterType,
                _ => "Unknown"
            };
        }
    }

    public class EditedFieldFormatter
    {
        public EditedFieldFormatter() : this("", "") { }
        protected EditedFieldFormatter(string name, string formatterType)
        {
            this.Name = name;
            this.FormatterType = formatterType;
        }

        protected EditedFieldFormatter(FieldFormatterModel fieldFormatter)
        {
            this.Name = fieldFormatter.Name;
            this.FormatterType = FieldFormatter.GetFormatterType(fieldFormatter);
        }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        public string FormatterType { get; set; }
    }

    public class EnumValue(int value, string stringValue)
    {
        public int Value { get; set; } = value;
        public string StringValue { get; set; } = stringValue;
    }

    public class EditedEnumFieldFormatter : EditedFieldFormatter
    {
        public EditedEnumFieldFormatter(string name, string formatterType, Dictionary<int, string> enumValues) : base(name, formatterType)
        {
            enumValues.ForEach(entry
                => EnumValues.Add(new EnumValue(entry.Key, entry.Value)));
        }

        public EditedEnumFieldFormatter(EnumFormatterModel enumFormatter) : base(enumFormatter)
        {
            enumFormatter.Values.ForEach(entry
                => EnumValues.Add(new EnumValue(entry.Key, entry.Value)));
        }

        public List<EnumValue> EnumValues { get; set; } = [];

        public Dictionary<int, string> GetEnumValues()
        {
            Dictionary<int, string> enumDict = [];
            EnumValues.ForEach(enumValue
                => enumDict.Add(enumValue.Value, enumValue.StringValue));
            return enumDict;
        }
    }
}
