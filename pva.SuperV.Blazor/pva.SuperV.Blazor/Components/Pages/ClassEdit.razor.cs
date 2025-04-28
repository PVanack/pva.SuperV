using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.Services;
using System.ComponentModel.DataAnnotations;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class ClassEdit
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        private IClassService ClassService { get; set; } = default!;
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
        private bool success;
        private bool isModification;

        private EditedClass EditedClass { get; set; } = default!;

        private List<string> BaseClasses { get; set; } = default!;

        protected override Task OnInitializedAsync()
        {
            isModification = !String.IsNullOrEmpty(ClassName) && State.EditedClass != null;
            pageTitle = isModification ? $"Class {State.EditedClass!.Name}" : "New class";
            EditedClass = new();
            if (isModification)
            {
                EditedClass = new(State.EditedClass!);
            }
            State.AddClassBreadcrumb(ProjectId, State.EditedClass);
            BaseClasses = GetBaseClasses();
            return base.OnInitializedAsync();
        }

        private List<string> GetBaseClasses()
        {
            List<ClassModel> allClasses = Task.Run(async () => await ClassService.GetClassesAsync(ProjectId)).Result;
            return [.. allClasses
                 .Where(clazz => clazz.Name != EditedClass.Name)
                 .Select(clazz => clazz.Name)];
        }

        private void OnFormatterTypeChanged(string selectedType)
        {
            EditedClass.BaseClassName = selectedType;
            StateHasChanged();
        }

        private async Task OnValidSubmit(EditContext context)
        {
            success = true;
            ClassModel clazz = new(EditedClass.Name, !String.IsNullOrWhiteSpace(EditedClass.BaseClassName) ? EditedClass.BaseClassName : null);
            if (isModification)
            {
                await ClassService.UpdateClassAsync(ProjectId, EditedClass.Name, clazz);
            }
            else
            {
                await ClassService.CreateClassAsync(ProjectId, clazz);
            }
            GoBackToHistoryRepositories();
        }

        private void CancelSubmit()
        {
            GoBackToHistoryRepositories();
        }

        private void GoBackToHistoryRepositories()
        {
            State.EditedClass = null;
            State.RemoveLastBreadcrumb();
            NavigationManager.NavigateTo($"/classes/{ProjectId}");
        }
    }

    public class EditedClass
    {
        public EditedClass() : this("", "") { }
        protected EditedClass(string name, string baseClassName)
        {
            this.Name = name;
            this.BaseClassName = baseClassName;
        }

        public EditedClass(ClassModel clazz)
        {
            this.Name = clazz.Name;
            this.BaseClassName = clazz.BaseClassName;
        }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        public string? BaseClassName { get; set; }
    }
}
