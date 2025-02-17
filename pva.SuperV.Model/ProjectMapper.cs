using pva.SuperV.Engine;

namespace pva.SuperV.Model
{
    public static class ProjectMapper
    {
        public static ProjectModel FromDto(CreateProjectRequest createProjectRequest)
        {
            return new ProjectModel("", createProjectRequest.Name, 0, createProjectRequest.Description, false);
        }

        public static ProjectModel ToDto(Project project)
        {
            return new ProjectModel(project.GetId(), project.Name!, project.Version, project.Description, project is RunnableProject);
        }
    }
}
