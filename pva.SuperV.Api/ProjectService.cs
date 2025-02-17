using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;

namespace pva.SuperV.Api
{
    public class ProjectService : IProjectService
    {
        public List<ProjectModel> GetProjects()
            => Project.Projects.Values
                .Select(project => ProjectMapper.ToDto(project))
                .ToList();

        public ProjectModel GetProject(string projectId)
        {
            Project? project = AccessHelpers.GetProject(projectId)
                ?? throw new UnknownEntityException("Project", projectId);
            return ProjectMapper.ToDto(project);

        }

        public ProjectModel CreateProject(CreateProjectRequest createProjectRequest)
        {
            WipProject wipProject = Project.CreateProject(createProjectRequest.Name);
            wipProject.Description = createProjectRequest.Description;
            return ProjectMapper.ToDto(wipProject);
        }

        public ProjectModel BuildProject(string projectId)
        {
            Project? project = AccessHelpers.GetProject(projectId)
                ?? throw new UnknownEntityException("Project", projectId);
            if (project is WipProject wipProject)
            {
                RunnableProject runnableProject = Project.Build(wipProject);
                return ProjectMapper.ToDto(runnableProject);
            }
            throw new NonWipProjectException(projectId);
        }

        public List<ClassModel> GetClasses(string projectId)
        {
            Project? project = AccessHelpers.GetProject(projectId)
                ?? throw new UnknownEntityException("Project", projectId);
            return project.Classes.Values
                .Select(clazz => ClassMapper.ToDto(clazz))
                .ToList();
        }

        public ClassModel GetClass(string projectId, string className)
        {
            Project? project = AccessHelpers.GetProject(projectId)
                ?? throw new UnknownEntityException("Project", projectId);
            if (!project.Classes.TryGetValue(className, out Class? clazz))
            {
                throw new UnknownEntityException("Class", className);
            }
            return ClassMapper.ToDto(clazz);
        }
    }
}
