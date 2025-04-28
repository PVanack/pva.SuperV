using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using pva.SuperV.Model.HistoryRepositories;
using pva.SuperV.Model.Services;
using System.ComponentModel.DataAnnotations;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class HistoryRepository
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        private IHistoryRepositoryService HistoryRepositoryService { get; set; } = default!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        private IDialogService DialogService { get; set; } = default!;
        [Inject]
        private State State { get; set; } = default!;

        [Parameter]
        public string ProjectId { get; set; } = default!;

        [Parameter]
        public string HistoryRepositoryName { get; set; } = default!;

        private string pageTitle = default!;
        private bool success;
        private bool isModification;

        private EditedHistoryRepository EditedHistoryRepository { get; set; } = default!;

        protected override Task OnInitializedAsync()
        {
            isModification = !String.IsNullOrEmpty(HistoryRepositoryName) && State.EditedHistoryRepository != null;
            pageTitle = isModification ? $"History respoitory {State.EditedHistoryRepository!.Name}" : "New history repository";
            EditedHistoryRepository = new();
            if (isModification)
            {
                EditedHistoryRepository = new(State.EditedHistoryRepository!);
            }
            State.AddHistoryRepositoryBreadcrumb(ProjectId, State.EditedHistoryRepository);
            return base.OnInitializedAsync();
        }

        private async Task OnValidSubmit(EditContext context)
        {
            success = true;
            HistoryRepositoryModel historyRepository = new(EditedHistoryRepository.Name);
            if (isModification)
            {
                await HistoryRepositoryService.UpdateHistoryRepositoryAsync(ProjectId, EditedHistoryRepository.Name, historyRepository);
            }
            else
            {
                await HistoryRepositoryService.CreateHistoryRepositoryAsync(ProjectId, historyRepository);
            }
            GoBackToHistoryRepositories();
        }

        private void CancelSubmit()
        {
            GoBackToHistoryRepositories();
        }

        private void GoBackToHistoryRepositories()
        {
            State.EditedHistoryRepository = null;
            State.RemoveLastBreadcrumb();
            NavigationManager.NavigateTo($"/history-repositories/{ProjectId}");
        }

    }

    public class EditedHistoryRepository
    {
        public EditedHistoryRepository() : this("") { }
        protected EditedHistoryRepository(string name)
        {
            this.Name = name;
        }

        public EditedHistoryRepository(HistoryRepositoryModel historyRepository)
        {
            this.Name = historyRepository.Name;
        }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }

}