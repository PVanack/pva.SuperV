using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace pva.SuperV.Blazor.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject]
        private State State { get; set; } = default!;
        private MudBreadcrumbs BreadcrumbsComponent { get; set; } = default!;

        private MudThemeProvider themeProvider = default!;
        private MudTheme theme = new();
        private bool isDarkMode = true;
        private bool drawerOpen = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                isDarkMode = await themeProvider.GetSystemPreference();
                State.MainlLayout = this;
                StateHasChanged();
            }
        }
        void ToggleDrawer()
        {
            drawerOpen = !drawerOpen;
        }

        public void Refresh()
        {
            StateHasChanged();
        }
    }
}
