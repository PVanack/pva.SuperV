using pva.SuperV.Engine;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Api.Mappers
{
    public static class ProjectMapper
    {
        public static ProjectModel ToDto(Project project)
            => new(project.GetId(), project.Name!, project.Version, project.Description, project is RunnableProject,
                !String.IsNullOrWhiteSpace(project.HistoryStorageEngineConnectionString));
    }
}
