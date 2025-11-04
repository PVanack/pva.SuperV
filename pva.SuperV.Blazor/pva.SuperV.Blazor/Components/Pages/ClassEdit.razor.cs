using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.Services;
using System.ComponentModel.DataAnnotations;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class ClassEdit
    {
        [Inject]
        private IClassService ClassService { get; set; } = default!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;
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

        private List<string> BaseClasses { get; set; } = [];

        protected async override Task OnInitializedAsync()
        {
            isModification = !String.IsNullOrEmpty(ClassName);
            EditedClass = new();
            if (isModification)
            {
                ClassModel clazz = await ClassService.GetClassAsync(ProjectId, ClassName);
                EditedClass = new(clazz);
                State.SetClassBreadcrumb(ProjectId, EditedClass!.Name);
            }
            pageTitle = isModification ? $"Class {EditedClass!.Name}" : "New class";
            BaseClasses = await GetBaseClasses();
            await base.OnInitializedAsync();
        }

        private async Task<List<string>> GetBaseClasses()
        {
            List<ClassModel> allClasses = await ClassService.GetClassesAsync(ProjectId);
            return [.. allClasses
                 .Where(clazz => clazz.Name != EditedClass.Name)
                 .Select(clazz => clazz.Name)];
        }

        private void OnFormatterTypeChanged(string selectedType)
        {
            EditedClass.BaseClassName = selectedType;
            StateHasChanged();
        }

        private async Task OnValidSubmit(EditContext _)
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
