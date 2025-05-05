using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.FieldFormatters;
using pva.SuperV.Model.Services;
using System.ComponentModel.DataAnnotations;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class FieldDefinition
    {
        public const string BooleanType = "Boolean";
        public const string DateTimeType = "Date & time";
        public const string DoubleType = "Double precision float";
        public const string FloatType = "Single precision float";
        public const string IntType = "Integer";
        public const string LongType = "Long integer";
        public const string ShortType = "Short integer";
        public const string StringType = "String";
        public const string TimeSpanType = "Time span";
        public const string UintType = "Unsigned integer";
        public const string UlongType = "Unsigned long integer";
        public const string UshortType = "Unsigned short integer";

        [Inject]
        private IFieldDefinitionService FieldDefinitionService { get; set; } = default!;
        [Inject]
        private IFieldFormatterService FieldFormatterService { get; set; } = default!;
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

        private static readonly Dictionary<string, Func<EditedFieldDefinition, EditedFieldDefinition>> fieldTypeToSubtype = new()
        {
            { BooleanType, (editedFieldDefinition) => new EditedFieldDefinition(editedFieldDefinition.Name, BooleanType, editedFieldDefinition.ValueFormatter, default(bool)) },
            { DateTimeType , (editedFieldDefinition) => new EditedFieldDefinition(editedFieldDefinition.Name, DateTimeType, editedFieldDefinition.ValueFormatter, default(DateTime)) },
            { DoubleType , (editedFieldDefinition) => new EditedFieldDefinition(editedFieldDefinition.Name, DoubleType, editedFieldDefinition.ValueFormatter, default(double)) },
            { FloatType, (editedFieldDefinition) => new EditedFieldDefinition(editedFieldDefinition.Name, FloatType, editedFieldDefinition.ValueFormatter, default(float)) },
            { IntType, (editedFieldDefinition) => new EditedFieldDefinition(editedFieldDefinition.Name, IntType, editedFieldDefinition.ValueFormatter, default(int)) },
            { LongType , (editedFieldDefinition) => new EditedFieldDefinition(editedFieldDefinition.Name, LongType, editedFieldDefinition.ValueFormatter, default(long)) },
            { ShortType , (editedFieldDefinition) => new EditedFieldDefinition(editedFieldDefinition.Name, ShortType, editedFieldDefinition.ValueFormatter, default(short)) },
            { StringType , (editedFieldDefinition) => new EditedFieldDefinition(editedFieldDefinition.Name, StringType, editedFieldDefinition.ValueFormatter, String.Empty) },
            { TimeSpanType , (editedFieldDefinition) => new EditedFieldDefinition(editedFieldDefinition.Name, TimeSpanType, editedFieldDefinition.ValueFormatter, default(TimeSpan)) },
            { UintType, (editedFieldDefinition) => new EditedFieldDefinition(editedFieldDefinition.Name, UintType, editedFieldDefinition.ValueFormatter, default(uint)) },
            { UlongType , (editedFieldDefinition) => new EditedFieldDefinition(editedFieldDefinition.Name, UlongType, editedFieldDefinition.ValueFormatter, default(ulong)) },
            { UshortType , (editedFieldDefinition) => new EditedFieldDefinition(editedFieldDefinition.Name, UshortType, editedFieldDefinition.ValueFormatter, default(ushort)) }
        };

        private static readonly Dictionary<string, Func<EditedFieldDefinition, FieldDefinitionModel>> fieldDefinitionToModel = new()
        {
            {BooleanType, (editedFieldDefinition)
                => new BoolFieldDefinitionModel(editedFieldDefinition!.Name, (bool)editedFieldDefinition!.DefaultValue!, editedFieldDefinition!.ValueFormatter)},
            {DateTimeType, (editedFieldDefinition)
                => new DateTimeFieldDefinitionModel(editedFieldDefinition!.Name, (DateTime)editedFieldDefinition!.DefaultValue!, editedFieldDefinition!.ValueFormatter)},
            {DoubleType, (editedFieldDefinition)
                => new DoubleFieldDefinitionModel(editedFieldDefinition!.Name, (double)editedFieldDefinition!.DefaultValue!, editedFieldDefinition!.ValueFormatter)},
            {FloatType, (editedFieldDefinition)
                => new FloatFieldDefinitionModel(editedFieldDefinition!.Name, (float)editedFieldDefinition!.DefaultValue!, editedFieldDefinition!.ValueFormatter)},
            {IntType, (editedFieldDefinition)
                => new IntFieldDefinitionModel(editedFieldDefinition!.Name, (int)editedFieldDefinition!.DefaultValue!, editedFieldDefinition!.ValueFormatter)},
            {LongType, (editedFieldDefinition)
                => new LongFieldDefinitionModel(editedFieldDefinition!.Name, (long)editedFieldDefinition!.DefaultValue!, editedFieldDefinition!.ValueFormatter)},
            {ShortType, (editedFieldDefinition)
                => new ShortFieldDefinitionModel(editedFieldDefinition!.Name, (short)editedFieldDefinition!.DefaultValue!, editedFieldDefinition!.ValueFormatter)},
            {StringType, (editedFieldDefinition)
                => new StringFieldDefinitionModel(editedFieldDefinition!.Name, (string)editedFieldDefinition!.DefaultValue!, editedFieldDefinition!.ValueFormatter)},
            {TimeSpanType, (editedFieldDefinition)
                => new TimeSpanFieldDefinitionModel(editedFieldDefinition!.Name, (TimeSpan)editedFieldDefinition!.DefaultValue!, editedFieldDefinition!.ValueFormatter)},
            {UintType, (editedFieldDefinition)
                => new UintFieldDefinitionModel(editedFieldDefinition!.Name, (uint)editedFieldDefinition!.DefaultValue!, editedFieldDefinition!.ValueFormatter)},
            {UlongType, (editedFieldDefinition)
                => new UlongFieldDefinitionModel(editedFieldDefinition!.Name, (ulong)editedFieldDefinition!.DefaultValue!, editedFieldDefinition!.ValueFormatter)},
            {UshortType , (editedFieldDefinition)
                => new UshortFieldDefinitionModel(editedFieldDefinition!.Name, (ushort)editedFieldDefinition!.DefaultValue!, editedFieldDefinition!.ValueFormatter)}
        };

        private static readonly Dictionary<Type, Func<FieldDefinitionModel, EditedFieldDefinition>> fieldFormatterFromModel = new()
        {
            {typeof(BoolFieldDefinitionModel) , (fieldDefinition) => new EditedFieldDefinition(fieldDefinition as BoolFieldDefinitionModel)},
            {typeof(DateTimeFieldDefinitionModel) , (fieldDefinition) => new EditedFieldDefinition(fieldDefinition as DateTimeFieldDefinitionModel)},
            {typeof(DoubleFieldDefinitionModel) , (fieldDefinition) => new EditedFieldDefinition(fieldDefinition as DoubleFieldDefinitionModel)},
            {typeof(FloatFieldDefinitionModel) , (fieldDefinition) => new EditedFieldDefinition(fieldDefinition as FloatFieldDefinitionModel)},
            {typeof(IntFieldDefinitionModel) , (fieldDefinition) => new EditedFieldDefinition(fieldDefinition as IntFieldDefinitionModel)},
            {typeof(LongFieldDefinitionModel) , (fieldDefinition) => new EditedFieldDefinition(fieldDefinition as LongFieldDefinitionModel)},
            {typeof(ShortFieldDefinitionModel) , (fieldDefinition) => new EditedFieldDefinition(fieldDefinition as ShortFieldDefinitionModel)},
            {typeof(StringFieldDefinitionModel) , (fieldDefinition) => new EditedFieldDefinition(fieldDefinition as StringFieldDefinitionModel)},
            {typeof(TimeSpanFieldDefinitionModel) , (fieldDefinition) => new EditedFieldDefinition(fieldDefinition as TimeSpanFieldDefinitionModel)},
            {typeof(UintFieldDefinitionModel) , (fieldDefinition) => new EditedFieldDefinition(fieldDefinition as UintFieldDefinitionModel)},
            {typeof(UlongFieldDefinitionModel) , (fieldDefinition) => new EditedFieldDefinition(fieldDefinition as UlongFieldDefinitionModel)},
            {typeof(UshortFieldDefinitionModel) , (fieldDefinition) => new EditedFieldDefinition(fieldDefinition as UshortFieldDefinitionModel)},
        };

        private string pageTitle = default!;
        private bool success;
        private bool isModification;

        private EditedFieldDefinition EditedFieldDefinition { get; set; } = default!;
        private FieldValue DefaultValue { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            isModification = !String.IsNullOrEmpty(FieldName);
            EditedFieldDefinition = new();
            if (isModification)
            {
                FieldDefinitionModel fieldDefinition = await FieldDefinitionService.GetFieldAsync(ProjectId, ClassName, FieldName);
                if (fieldFormatterFromModel.TryGetValue(fieldDefinition.GetType(), out var createFunc))
                {
                    EditedFieldDefinition = createFunc(fieldDefinition);
                }
                State.SetFieldDefinitionBreadcrumb(ProjectId, ClassName, EditedFieldDefinition.Name);
            }
            pageTitle = isModification ? $"{EditedFieldDefinition.FieldType} {EditedFieldDefinition!.Name}" : "New field";
            await base.OnInitializedAsync();
        }

        private List<FieldFormatterModel> GetFieldFormatters()
        {
            return Task.Run(async () => await FieldFormatterService.GetFieldFormattersAsync(ProjectId)).Result;
        }


        private async Task OnValidSubmit(EditContext context)
        {
            success = true;
            FieldDefinitionModel fieldDefinition = MapFieldDefinition(EditedFieldDefinition, DefaultValue);
            if (isModification)
            {
                await FieldDefinitionService.UpdateFieldAsync(ProjectId, ClassName, EditedFieldDefinition.Name, fieldDefinition);
            }
            else
            {
                await FieldDefinitionService.CreateFieldsAsync(ProjectId, ClassName, [fieldDefinition]);
            }
            GoBackToFieldDefinitions();
        }

        private void CancelSubmit()
        {
            GoBackToFieldDefinitions();
        }

        private void GoBackToFieldDefinitions()
        {
            NavigationManager.NavigateTo($"/field-definitions/{ProjectId}/{ClassName}");
        }

        private static List<string> GetFieldTypes()
        {
            return [.. fieldTypeToSubtype.Keys];
        }

        private void OnValueFormatterChanged(string valueFormatter)
        {
            EditedFieldDefinition.ValueFormatter = valueFormatter;
        }

        private void OnFieldTypeChanged(string selectedType)
        {
            if (!String.IsNullOrEmpty(selectedType) && fieldTypeToSubtype.TryGetValue(selectedType, out var createFunc))
            {
                EditedFieldDefinition = createFunc(EditedFieldDefinition);
            }
            StateHasChanged();
        }

        private static FieldDefinitionModel MapFieldDefinition(EditedFieldDefinition editedFieldDefinition, FieldValue defaultValue)
        {
            editedFieldDefinition.DefaultValue = defaultValue.Value;
            if (fieldDefinitionToModel.TryGetValue(editedFieldDefinition.FieldType, out var mapFunc))
            {
                return mapFunc(editedFieldDefinition);
            }
            throw new InvalidOperationException($"No mapping for field definition type {editedFieldDefinition.GetType()}");
        }
    }

    public class EditedFieldDefinition(string name, string fieldType, string? valueFormatter, object? defaultValue)
    {
        public EditedFieldDefinition() : this("", "", null, null) { }

        public EditedFieldDefinition(BoolFieldDefinitionModel? field) : this(field!.Name, FieldDefinition.BooleanType, field!.ValueFormatter, field!.DefaultValue) { }
        public EditedFieldDefinition(DateTimeFieldDefinitionModel? field) : this(field!.Name, FieldDefinition.DateTimeType, field!.ValueFormatter, field!.DefaultValue) { }
        public EditedFieldDefinition(DoubleFieldDefinitionModel? field) : this(field!.Name, FieldDefinition.DoubleType, field!.ValueFormatter, field!.DefaultValue) { }
        public EditedFieldDefinition(FloatFieldDefinitionModel? field) : this(field!.Name, FieldDefinition.FloatType, field!.ValueFormatter, field!.DefaultValue) { }
        public EditedFieldDefinition(IntFieldDefinitionModel? field) : this(field!.Name, FieldDefinition.IntType, field!.ValueFormatter, field!.DefaultValue) { }
        public EditedFieldDefinition(LongFieldDefinitionModel? field) : this(field!.Name, FieldDefinition.LongType, field!.ValueFormatter, field!.DefaultValue) { }
        public EditedFieldDefinition(ShortFieldDefinitionModel? field) : this(field!.Name, FieldDefinition.ShortType, field!.ValueFormatter, field!.DefaultValue) { }
        public EditedFieldDefinition(StringFieldDefinitionModel? field) : this(field!.Name, FieldDefinition.StringType, field!.ValueFormatter, field!.DefaultValue) { }
        public EditedFieldDefinition(TimeSpanFieldDefinitionModel? field) : this(field!.Name, FieldDefinition.TimeSpanType, field!.ValueFormatter, field!.DefaultValue) { }
        public EditedFieldDefinition(UintFieldDefinitionModel? field) : this(field!.Name, FieldDefinition.UintType, field!.ValueFormatter, field!.DefaultValue) { }
        public EditedFieldDefinition(UlongFieldDefinitionModel? field) : this(field!.Name, FieldDefinition.UlongType, field!.ValueFormatter, field!.DefaultValue) { }
        public EditedFieldDefinition(UshortFieldDefinitionModel? field) : this(field!.Name, FieldDefinition.UshortType, field!.ValueFormatter, field!.DefaultValue) { }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; } = name;
        public string FieldType { get; set; } = fieldType;
        public string? ValueFormatter { get; set; } = valueFormatter ?? "";
        public object? DefaultValue { get; set; } = defaultValue;
    }
}