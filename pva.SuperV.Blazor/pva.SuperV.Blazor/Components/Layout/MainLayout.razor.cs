using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace pva.SuperV.Blazor.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase
    {
        MudTheme theme = new();
        bool isDarkMode = true;
        bool drawerOpen = false;

        void ToggleDrawer()
        {
            drawerOpen = !drawerOpen;
        }
    }
}
