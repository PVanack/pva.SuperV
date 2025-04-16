using Microsoft.AspNetCore.Components;

namespace pva.SuperV.Blazor.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase
    {
        bool _drawerOpen = true;

        void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }
    }
}
