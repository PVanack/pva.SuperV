using pva.SuperV.Engine;

namespace pva.SuperV.Api
{
    public static class AccessHelpers
    {
        public static Project? GetProject(string projectId) =>
            Project.Projects.TryGetValue(projectId, out Project? project)
                ? project
                : null;
    }
}
