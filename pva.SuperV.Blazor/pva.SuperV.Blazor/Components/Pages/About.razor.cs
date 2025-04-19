namespace pva.SuperV.Blazor.Components.Pages
{

    public partial class About
    {
        protected override void OnInitialized()
        {
            State.SetAboutBreadcrumb();
            base.OnInitialized();
        }
    }
}