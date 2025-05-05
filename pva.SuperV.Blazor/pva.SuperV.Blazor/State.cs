using MudBlazor;
using pva.SuperV.Blazor.Components.Layout;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Blazor
{
    public class State
    {
        public List<BreadcrumbItem> Breadcrumbs { get; set; } = default!;
        public MainLayout MainlLayout { get; set; } = default!;
        public ProjectModel? CurrentProject { get; set; } = default!;

        internal void SetHomeBreadCrumb(bool refreshBreadcrumbs = true)
        {
            Breadcrumbs = [new("Home", "#")];
            RefreshBreadcrumbsIfNeeded(refreshBreadcrumbs);
        }

        internal void SetAboutBreadcrumb(bool refreshBreadcrumbs = true)
        {
            SetHomeBreadCrumb(false);
            Breadcrumbs.Add(new("About", "/about"));
            RefreshBreadcrumbsIfNeeded(refreshBreadcrumbs);

        }

        internal void SetProjectsBreadcrumb(bool refreshBreadcrumbs = true)
        {
            SetHomeBreadCrumb(false);
            Breadcrumbs.Add(new("Projects", "/projects"));
            RefreshBreadcrumbsIfNeeded(refreshBreadcrumbs);
        }

        internal void SetProjectBreadcrumb(ProjectModel? project, bool refreshBreadcrumbs = true)
        {
            SetProjectsBreadcrumb(refreshBreadcrumbs);
            if (project != null)
            {
                Breadcrumbs.Add(new(project.Name, $"/project/{project.Id}"));
            }
            RefreshBreadcrumbsIfNeeded();
        }

        internal void SetFieldFormattersBreadcrumb(string projectId, bool refreshBreadCrumbs = true)
        {
            SetProjectBreadcrumb(CurrentProject, refreshBreadCrumbs);
            Breadcrumbs.Add(new("Field formatters", $"/field-formatters/{projectId}"));
            RefreshBreadcrumbsIfNeeded();
        }

        internal void SetFieldFormatterBreadcrumb(string projectId, string fieldFormatterName)
        {
            SetFieldFormattersBreadcrumb(projectId, false);
            Breadcrumbs.Add(new(fieldFormatterName, $"/field-formatter/{projectId}/{fieldFormatterName}"));
            RefreshBreadcrumbsIfNeeded();
        }

        internal void SetHistoryRepositoriesBreadcrumb(string projectId, bool refreshBreadCrumbs = true)
        {
            SetProjectBreadcrumb(CurrentProject, refreshBreadCrumbs);
            Breadcrumbs.Add(new("History repositories", $"/history-repositories/{projectId}"));
            RefreshBreadcrumbsIfNeeded();
        }

        internal void SetHistoryRepositoryBreadcrumb(string projectId, string historyRepositoryName)
        {
            SetHistoryRepositoriesBreadcrumb(projectId);
            Breadcrumbs.Add(new(historyRepositoryName, $"/history-repositories/{projectId}/{historyRepositoryName}"));
            RefreshBreadcrumbsIfNeeded();
        }

        internal void SetClassesBreadcrumb(string projectId, bool refreshBreadCrumbs = true)
        {
            SetProjectBreadcrumb(CurrentProject, refreshBreadCrumbs);
            Breadcrumbs.Add(new("Classes", $"/classes/{projectId}"));
            RefreshBreadcrumbsIfNeeded(refreshBreadCrumbs);
        }

        internal void SetClassBreadcrumb(string projectId, string className, bool refreshBreadCrumbs = true)
        {
            if (className != null)
            {
                SetClassesBreadcrumb(projectId, false);
                Breadcrumbs.Add(new(className, $"/class/{projectId}/{className}"));
                RefreshBreadcrumbsIfNeeded(refreshBreadCrumbs);
            }
        }

        internal void SetFieldDefinitionsBreadcrumb(string projectId, string className, bool refreshBreadCrumbs = true)
        {
            SetClassBreadcrumb(projectId, className, false);
            Breadcrumbs.Add(new("Fields", $"/field-definitions/{projectId}/{className}"));
            RefreshBreadcrumbsIfNeeded(refreshBreadCrumbs);
        }

        internal void SetFieldDefinitionBreadcrumb(string projectId, string className, string fieldDefinitionName, bool refreshBreadCrumbs = true)
        {
            if (fieldDefinitionName != null)
            {
                SetFieldDefinitionsBreadcrumb(projectId, className, false);
                Breadcrumbs.Add(new(fieldDefinitionName, $"/field-definition/{projectId}/{className}/{fieldDefinitionName}"));
                RefreshBreadcrumbsIfNeeded(refreshBreadCrumbs);
            }
        }

        internal void SetInstancesBreadcrumb(string projectId, bool refreshBreadCrumbs = true)
        {
            SetProjectBreadcrumb(CurrentProject, refreshBreadCrumbs);
            Breadcrumbs.Add(new("Instances", $"/instances/{projectId}"));
            RefreshBreadcrumbsIfNeeded(refreshBreadCrumbs);
        }

        internal void SetInstanceBreadcrumb(string projectId, string instanceName)
        {
            SetInstancesBreadcrumb(projectId, false);
            Breadcrumbs.Add(new(instanceName, $"/instance/{projectId}/{instanceName}"));
            RefreshBreadcrumbsIfNeeded();
        }

        internal void SetFieldProcessingsBreadcrumb(string projectId, string className, string fieldName, bool refreshBreadCrumbs = true)
        {
            if (fieldName != null)
            {
                SetFieldDefinitionBreadcrumb(projectId, className, fieldName, false);
                Breadcrumbs.Add(new("Field value processings", $"/field-processings/{projectId}/{className}/{fieldName}"));
                RefreshBreadcrumbsIfNeeded(refreshBreadCrumbs);
            }
        }

        internal void SetFieldProcessingBreadcrumb(string projectId, string className, string fieldName, string processingName)
        {
            if (fieldName != null)
            {
                SetFieldProcessingsBreadcrumb(projectId, className, fieldName, false);
                Breadcrumbs.Add(new(processingName, $"/field-processing/{projectId}/{className}/{fieldName}/{processingName}"));
                RefreshBreadcrumbsIfNeeded(true);
            }
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
