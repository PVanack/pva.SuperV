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
            { EnumFormatterType , (editedFieldFormatter) => new EditedEnumFieldFormatter(editedFieldFormatter.Name, editedFieldFormatter.FormatterType, new Dictionary<int, string>()) }
        };

        private static readonly Dictionary<Type, Func<EditedFieldFormatter, FieldFormatterModel>> fieldFormatterToModel = new()
        {
            {typeof(EditedEnumFieldFormatter) , (editedFieldFormatter) => CreateEnumFormatterModel(editedFieldFormatter)}
        };

        private static readonly Dictionary<Type, Func<FieldFormatterModel, EditedFieldFormatter>> fieldFormatterFromModel = new()
        {
            {typeof(EnumFormatterModel) , (fieldFormatter) => CreateEditableEnumFormatter(fieldFormatter)}
        };

        private static FieldFormatterModel CreateEnumFormatterModel(EditedFieldFormatter editedFieldFormatter)
        {
            EditedEnumFieldFormatter enumFormatter = editedFieldFormatter as EditedEnumFieldFormatter;
            return new EnumFormatterModel(editedFieldFormatter.Name, enumFormatter.GetEnumValues());
        }

        private static EditedFieldFormatter CreateEditableEnumFormatter(FieldFormatterModel fieldFormatter)
        {
            EnumFormatterModel enumFormatter = fieldFormatter as EnumFormatterModel;
            return new EditedEnumFieldFormatter(enumFormatter);
        }

        private string pageTitle = default!;
        private bool success;
        private bool isModification;

        private EditedFieldFormatter EditedFieldFormatter { get; set; } = default!;

        protected override Task OnInitializedAsync()
        {
            isModification = !String.IsNullOrEmpty(FieldFormatterName) && State.EditedFieldFormatter != null;
            pageTitle = isModification ? $"{GetFormatterType(State.EditedFieldFormatter)} {State.EditedFieldFormatter.Name}" : "New field formatter";
            EditedFieldFormatter = new();
            if (isModification)
            {
                if (fieldFormatterFromModel.TryGetValue(State.EditedFieldFormatter.GetType(), out var createFunc))
                {
                    EditedFieldFormatter = createFunc(State.EditedFieldFormatter);
                }
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
                CreateFieldFormatterRequest createRequest = new CreateFieldFormatterRequest(fieldFormatter);
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

        private List<string> GetFieldFormatterTypes()
        {
            return fieldFormatterToSubtype.Keys.ToList();
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
        private FieldFormatterModel MapFieldFormatter(EditedFieldFormatter editedFieldFormatter)
        {
            if (fieldFormatterToModel.TryGetValue(editedFieldFormatter.GetType(), out var mapFunc))
            {
                return mapFunc(editedFieldFormatter);
            }
            throw new InvalidOperationException($"No mapping for field formatter type {editedFieldFormatter.GetType()}");
        }

        public static string GetFormatterType(FieldFormatterModel fieldFormatter)
        {
            return fieldFormatter switch
            {
                EnumFormatterModel => FieldFormatter.EnumFormatterType,
                _ => "Unknown"
            };
        }
    }

    public class EditedFieldFormatter
    {
        private string name;
        private string formatterType;
        public EditedFieldFormatter() : this("", "") { }
        protected EditedFieldFormatter(string name, string formatterType)
        {
            this.name = name;
            this.formatterType = formatterType;
        }

        protected EditedFieldFormatter(FieldFormatterModel fieldFormatter)
        {
            this.name = fieldFormatter.Name;
            this.formatterType = FieldFormatter.GetFormatterType(fieldFormatter);
        }

        [Required(AllowEmptyStrings = false)]
        public string Name { get => name; set => name = value; }
        public string FormatterType { get => formatterType; set => formatterType = value; }
    }

    public class EnumValue
    {
        private int value;
        private string stringValue;

        public EnumValue(int value, string stringValue)
        {
            this.value = value;
            this.stringValue = stringValue;
        }

        public int Value { get => value; set => this.value = value; }
        public string StringValue { get => stringValue; set => this.stringValue = value; }
    }

    public class EditedEnumFieldFormatter : EditedFieldFormatter
    {
        private List<EnumValue> enumValues = [];
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

        public List<EnumValue> EnumValues { get => enumValues; set => enumValues = value; }

        public Dictionary<int, string> GetEnumValues()
        {
            Dictionary<int, string> enumDict = [];
            enumValues.ForEach(enumValue => enumDict.Add(enumValue.Value, enumValue.StringValue));
            return enumDict;
        }
    }
}
