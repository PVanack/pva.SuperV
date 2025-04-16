using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using pva.SuperV.Blazor.SuperVClient;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class Projects
    {
        [Inject]
        private RestClient RestClient { get; set; } = default!;
        private readonly HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;
        private readonly bool hidePageNumber = false;
        private readonly bool hidePagination = false;
        private readonly bool hideRowsPerPage = false;
        private readonly string rowsPerPageString = "Rows per page:";
        private readonly string infoFormat = "{first_item}-{last_item} of {all_items}";
        private readonly string allItemsText = "All";

        private IEnumerable<ProjectModel>? projects = [];

        protected override async Task OnInitializedAsync()
        {
            projects = await RestClient.GetProjectsAsync();
            //await Task.Run(() =>
            //    projects = new List<ProjectModel>() {
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = false },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "RP1", Description = "Runnable project", Runnable = true },
            //    new ProjectModel() { Name = "WIP", Description = "WIP project", Runnable = false }
            //    }.AsEnumerable()
            //);
        }

        private async Task CreateProject(MouseEventArgs e)
        {
            CreateProjectRequest request = new()
            { Name = "Name", Description = "Description", HistoryStorageConnectionString = "" };
            ProjectModel createdProject = await RestClient.CreateBlankProjectAsync(request);
        }

    }
}
