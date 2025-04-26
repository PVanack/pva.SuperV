using MudBlazor;
using pva.SuperV.Blazor.Components.Layout;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Blazor
{
    public class State
    {
        public List<BreadcrumbItem> Breadcrumbs { get; set; } = default!;
        public ProjectModel? EditedProject { get; set; }
        public MainLayout MainlLayout { get; set; } = default!;

        public void SetHomeBreadCrumb(bool refreshBreadcrumbs = true)
        {
            Breadcrumbs = [new("Home", "#")];
            RefreshBreadcrumbsIfNeeded(refreshBreadcrumbs);
        }

        public void SetProjectsBreadcrumb(bool refreshBreadcrumbs = true)
        {
            SetHomeBreadCrumb(false);
            Breadcrumbs.Add(new("Projects", "#/projects"));
            RefreshBreadcrumbsIfNeeded(refreshBreadcrumbs);
        }

        public void AddProjectBreadcrumb(ProjectModel? project, bool refreshBreadcrumbs = true)
        {
            SetProjectsBreadcrumb(false);
            if (project != null)
            {
                Breadcrumbs.Add(new(project.Name, $"#/project/{project.Id}"));
            }
            RefreshBreadcrumbsIfNeeded();
        }

        public void SetAboutBreadcrumb(bool refreshBreadcrumbs = true)
        {
            SetHomeBreadCrumb(false);
            Breadcrumbs.Add(new("About", "#/about"));
            RefreshBreadcrumbsIfNeeded(refreshBreadcrumbs);

        }

        public void RemoveLastBreadcrumb()
        {
            if (Breadcrumbs.Count > 0)
            {
                Breadcrumbs.RemoveAt(Breadcrumbs.Count - 1);
            }
            RefreshBreadcrumbsIfNeeded();
        }

        private void RefreshBreadcrumbsIfNeeded(bool refreshBreadcrumbs = true)
        {
            if (refreshBreadcrumbs)
            {
                MainlLayout?.Refresh();
            }
        }
    }
}
