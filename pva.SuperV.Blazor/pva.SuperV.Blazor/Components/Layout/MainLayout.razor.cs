using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace pva.SuperV.Blazor.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase
    {
        private MudThemeProvider themeProvider = default!;
        private MudTheme theme = new();
        private bool isDarkMode = true;
        private bool drawerOpen = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                isDarkMode = await themeProvider.GetSystemPreference();
                StateHasChanged();
            }
        }
        void ToggleDrawer()
        {
            drawerOpen = !drawerOpen;
        }
    }
}
