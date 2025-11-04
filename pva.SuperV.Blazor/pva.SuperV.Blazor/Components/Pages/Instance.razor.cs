using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.Instances;
using pva.SuperV.Model.Services;
using System.ComponentModel.DataAnnotations;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class Instance
    {
        [Inject]
        private IInstanceService InstanceService { get; set; } = default!;
        [Inject]
        private IFieldValueService FieldValueService { get; set; } = default!;
        [Inject]
        private IClassService ClassService { get; set; } = default!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        private State State { get; set; } = default!;

        [Parameter]
        public string ProjectId { get; set; } = default!;

        [Parameter]
        public string InstanceName { get; set; } = default!;

        private string pageTitle = default!;
        private bool success;
        private bool isModification;
        private List<string> ClassNames { get; set; } = [];
        private Dictionary<string, FieldValue> FieldValues { get; set; } = [];
        private EditedInstance EditedInstance { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            isModification = !String.IsNullOrEmpty(InstanceName);
            EditedInstance = new();
            if (isModification)
            {
                InstanceModel instance = await InstanceService.GetInstanceAsync(ProjectId, InstanceName);
                EditedInstance = new(instance);

                State.SetInstanceBreadcrumb(ProjectId, EditedInstance.Name);
            }
            pageTitle = isModification ? $"{EditedInstance!.Name}" : "New instance";
            ClassNames = await GetClassNames();
            await base.OnInitializedAsync();
        }

        private async Task OnValidSubmit(EditContext _)
        {
            success = true;
            List<FieldModel> fields = [];
            EditedInstance.Fields.ForEach(field
                => fields.Add(BuildFieldWithValue(field.FieldModel, FieldValues[field.Name].Value))
            );
            InstanceModel instance = new(EditedInstance.Name, EditedInstance.ClassName, fields);
            if (isModification)
            {
                fields.ForEach(field
                    => FieldValueService.UpdateFieldValueAsync(ProjectId, instance.Name, field.Name, field.FieldValue));
            }
            else
            {
                await InstanceService.CreateInstanceAsync(ProjectId, instance, true);
            }
            GoBackToInstances();
        }

        private static FieldModel BuildFieldWithValue(FieldModel fieldModel, object? fieldValue)
        {
            FieldValueModel fieldValueModel = fieldModel.FieldValue switch
            {
                BoolFieldValueModel typedActual => new BoolFieldValueModel((bool)fieldValue!, typedActual.FormattedValue, typedActual.Quality, typedActual.Timestamp),
                DateTimeFieldValueModel typedActual => new DateTimeFieldValueModel((DateTime)fieldValue!, typedActual.FormattedValue, typedActual.Quality, typedActual.Timestamp),
                DoubleFieldValueModel typedActual => new DoubleFieldValueModel((double)fieldValue!, typedActual.FormattedValue, typedActual.Quality, typedActual.Timestamp),
                FloatFieldValueModel typedActual => new FloatFieldValueModel((float)fieldValue!, typedActual.FormattedValue, typedActual.Quality, typedActual.Timestamp),
                IntFieldValueModel typedActual => new IntFieldValueModel((int)fieldValue!, typedActual.FormattedValue, typedActual.Quality, typedActual.Timestamp),
                LongFieldValueModel typedActual => new LongFieldValueModel((long)fieldValue!, typedActual.FormattedValue, typedActual.Quality, typedActual.Timestamp),
                ShortFieldValueModel typedActual => new ShortFieldValueModel((short)fieldValue!, typedActual.FormattedValue, typedActual.Quality, typedActual.Timestamp),
                StringFieldValueModel typedActual => new StringFieldValueModel((string)fieldValue!, typedActual.Quality, typedActual.Timestamp),
                TimeSpanFieldValueModel typedActual => new TimeSpanFieldValueModel((TimeSpan)fieldValue!, typedActual.FormattedValue, typedActual.Quality, typedActual.Timestamp),
                UintFieldValueModel typedActual => new UintFieldValueModel((uint)fieldValue!, typedActual.FormattedValue, typedActual.Quality, typedActual.Timestamp),
                UlongFieldValueModel typedActual => new UlongFieldValueModel((ulong)fieldValue!, typedActual.FormattedValue, typedActual.Quality, typedActual.Timestamp),
                UshortFieldValueModel typedActual => new UshortFieldValueModel((ushort)fieldValue!, typedActual.FormattedValue, typedActual.Quality, typedActual.Timestamp),
                _ => throw new NotImplementedException(),
            };
            return new FieldModel(fieldModel.Name, fieldModel.Type, fieldValueModel);
        }

        private void CancelSubmit()
        {
            GoBackToInstances();
        }

        private void GoBackToInstances()
        {
            NavigationManager.NavigateTo($"/instances/{ProjectId}");
        }

        private async Task<List<string>> GetClassNames()
        {
            List<ClassModel> classes = await ClassService.GetClassesAsync(ProjectId);
            return [.. classes.Select(c => c.Name)];
        }

        private async Task OnClassNameChanged(string selectedClass)
        {
            if (!String.IsNullOrEmpty(selectedClass))
            {
                InstanceModel instance = new(EditedInstance.Name, selectedClass, []);
                instance = await InstanceService.CreateInstanceAsync(ProjectId, instance, false);
                EditedInstance = new(instance);
                FieldValues.Clear();
                EditedInstance.Fields.ForEach(field
                    => FieldValues.Add(field.Name, new FieldValue()));

            }
            StateHasChanged();
        }
    }

    public class InstanceField(FieldModel field)
    {
        public string Name { get; set; } = field.Name;
        public object? Value { get; set; } = GetFieldValue(field.FieldValue);
        public FieldModel FieldModel { get; set; } = field;

        public static object? GetFieldValue(FieldValueModel field)
        {
            return field switch
            {
                BoolFieldValueModel subTypeField => subTypeField.Value,
                DateTimeFieldValueModel subTypeField => subTypeField.Value,
                DoubleFieldValueModel subTypeField => subTypeField.Value,
                FloatFieldValueModel subTypeField => subTypeField.Value,
                IntFieldValueModel subTypeField => subTypeField.Value,
                LongFieldValueModel subTypeField => subTypeField.Value,
                ShortFieldValueModel subTypeField => subTypeField.Value,
                StringFieldValueModel subTypeField => subTypeField.Value,
                TimeSpanFieldValueModel subTypeField => subTypeField.Value,
                UintFieldValueModel subTypeField => subTypeField.Value,
                UlongFieldValueModel subTypeField => subTypeField.Value,
                UshortFieldValueModel subTypeField => subTypeField.Value,
                _ => null
            };
        }
    }

    public class EditedInstance(string name, string className, List<FieldModel> fields)
    {
        public EditedInstance() : this("", "", []) { }
        public EditedInstance(InstanceModel instance) : this(instance.Name, instance.ClassName, instance.Fields) { }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; } = name;
        public string ClassName { get; set; } = className;
        public List<InstanceField> Fields { get; set; } = MapFieldsModel(fields);

        private static List<InstanceField> MapFieldsModel(List<FieldModel> fields)
        {
            return [.. fields.Select(field => new InstanceField(field))];
        }

    }

}
