using MudBlazor;
using pva.SuperV.Blazor.Components.Layout;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.FieldFormatters;
using pva.SuperV.Model.HistoryRepositories;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Blazor
{
    public class State
    {
        public List<BreadcrumbItem> Breadcrumbs { get; set; } = default!;
        public ProjectModel? EditedProject { get; set; } = default!;
        public FieldFormatterModel? EditedFieldFormatter { get; set; } = default!;
        public HistoryRepositoryModel? EditedHistoryRepository { get; set; } = default!;
        public ClassModel? EditedClass { get; set; } = default!;
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
            SetProjectsBreadcrumb(refreshBreadcrumbs);
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

        internal void AddFieldFormattersBreadcrumb(string projectId, bool refreshBreadCrumbs = true)
        {
            AddProjectBreadcrumb(EditedProject, refreshBreadCrumbs);
            Breadcrumbs.Add(new("Field formatters", $"/field-formatters/{projectId}"));
            RefreshBreadcrumbsIfNeeded();
        }

        internal void AddFieldFormatterBreadcrumb(string projectId, FieldFormatterModel? editedFieldFormatter)
        {
            if (editedFieldFormatter != null)
            {
                Breadcrumbs.Add(new($"Field formatter {editedFieldFormatter.Name}", $"/field-formatter/{projectId}/{editedFieldFormatter?.Name}"));
                RefreshBreadcrumbsIfNeeded();
            }
        }

        internal void AddHistoryRepositoriesBreadcrumb(string projectId, bool refreshBreadCrumbs = true)
        {
            AddProjectBreadcrumb(EditedProject, refreshBreadCrumbs);
            Breadcrumbs.Add(new("History repositories", $"/history-repositories/{projectId}"));
            RefreshBreadcrumbsIfNeeded();
        }

        internal void AddHistoryRepositoryBreadcrumb(string projectId, HistoryRepositoryModel? historyRepository)
        {
            if (historyRepository != null)
            {
                Breadcrumbs.Add(new($"History repository {historyRepository.Name}", $"/history-repositories/{projectId}/{historyRepository?.Name}"));
                RefreshBreadcrumbsIfNeeded();
            }
        }
        internal void AddClassesBreadcrumb(string projectId, bool refreshBreadCrumbs = true)
        {
            AddProjectBreadcrumb(EditedProject, refreshBreadCrumbs);
            Breadcrumbs.Add(new("Classes", $"/classes/{projectId}"));
            RefreshBreadcrumbsIfNeeded();
        }

        internal void AddClassBreadcrumb(string projectId, ClassModel? editedClass)
        {
            if (editedClass != null)
            {
                Breadcrumbs.Add(new($"Class {editedClass.Name}", $"/class/{projectId}/{editedClass?.Name}"));
                RefreshBreadcrumbsIfNeeded();
            }
        }

    }
}
