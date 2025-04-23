using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace pva.SuperV.Blazor.Components.Dialogs
{
    public partial class DeleteConfirmationDialog
    {
        [Parameter]
        public string EntityDescription { get; set; } = default!;

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = default!;

        private void Submit() => MudDialog.Close(DialogResult.Ok(true));

        private void Cancel() => MudDialog.Cancel();
    }
}
