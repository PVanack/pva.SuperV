using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;
using System.ComponentModel.DataAnnotations;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class ScriptDefinition
    {
        [Inject]
        private IProjectService ProjectService { get; set; } = default!;
        [Inject]
        private IScriptService ScriptService { get; set; } = default!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        private State State { get; set; } = default!;

        [Parameter]
        public string ProjectId { get; set; } = default!;

        [Parameter]
        public string ScriptName { get; set; } = default!;

        private string pageTitle = default!;
        private bool success;
        private bool isModification;
        private EditedScript EditedScript { get; set; } = default!;

        private HashSet<string> TopicNames { get; set; } = [];

        protected async override Task OnInitializedAsync()
        {
            isModification = !String.IsNullOrEmpty(ScriptName);
            EditedScript = new();
            if (isModification)
            {
                ScriptDefinitionModel script = await ScriptService.GetScriptAsync(ProjectId, ScriptName);
                EditedScript = new(script);
                State.SetScriptBreadcrumb(ProjectId, EditedScript!.Name);
            }
            pageTitle = isModification ? $"Script {EditedScript!.Name}" : "New script";
            TopicNames = await ProjectService.GetProjectTopicNames(ProjectId);
            await base.OnInitializedAsync();
        }

        private void OnTopicNameChanged(string selectedType)
        {
            EditedScript.TopicName = selectedType;
            StateHasChanged();
        }

        private async Task OnValidSubmit(EditContext _)
        {
            success = true;
            ScriptDefinitionModel script = new(EditedScript.Name, EditedScript.TopicName, EditedScript.Source);
            if (isModification)
            {
                await ScriptService.UpdateScriptAsync(ProjectId, EditedScript.Name, script);
            }
            else
            {
                await ScriptService.CreateScriptAsync(ProjectId, script);
            }
            GoBackToScripts();
        }

        private void CancelSubmit()
        {
            GoBackToScripts();
        }

        private void GoBackToScripts()
        {
            NavigationManager.NavigateTo($"/scripts/{ProjectId}");
        }
    }

    public class EditedScript
    {
        public EditedScript() : this("", "", "") { }
        protected EditedScript(string name, string topicName, string source)
        {
            this.Name = name;
            this.TopicName = topicName;
            this.Source = source;
        }

        public EditedScript(ScriptDefinitionModel script)
        {
            this.Name = script.Name;
            this.TopicName = script.TopicName;
            this.Source = script.Source;
        }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string TopicName { get; set; }
        public string Source { get; set; }
    }
}
