using Microsoft.AspNetCore.Components;

namespace pva.SuperV.Blazor.Client.Pages
{
    public partial class Dismissible
    {
        private bool show;
        [Parameter]
        public bool Show
        {
            get => show;
            set
            {
                if (value != show)
                {
                    show = value;
                    ShowChanged?.Invoke(show);
                }
            }
        }
        [Parameter]
        public Action<bool>? ShowChanged { get; set; }
        [Parameter]
        public RenderFragment ChildContent { get; set; } = default!;
        public void Dismiss()
        => Show = false;
    }
}
