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
        private readonly MudTheme theme = new();
        private bool isDarkMode = true;
        private bool drawerOpen;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                isDarkMode = await themeProvider.GetSystemDarkModeAsync();
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
