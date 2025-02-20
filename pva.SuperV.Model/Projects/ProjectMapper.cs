using pva.SuperV.Engine;

namespace pva.SuperV.Model.Projects
{
    public static class ProjectMapper
    {
        public static ProjectModel ToDto(Project project)
        {
            return new ProjectModel(project.GetId(), project.Name!, project.Version, project.Description, project is RunnableProject);
        }
    }
}
