using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.Projects;

namespace pva.SuperV.Api.Services.Classes
{
    public class ClassService : BaseService, IClassService
    {
        public List<ClassModel> GetClasses(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            return project.Classes.Values
                .Select(clazz => ClassMapper.ToDto(clazz))
                .ToList();
        }

        public ClassModel GetClass(string projectId, string className)
        {
            Class clazz = GetClassEntity(projectId, className);
            return ClassMapper.ToDto(clazz);
        }

        public ClassModel CreateClass(string projectId, ClassModel createRequest)
        {
            Project project = GetProjectEntity(projectId);
            if (project is WipProject wipProject)
            {
                return ClassMapper.ToDto(wipProject.AddClass(createRequest!.Name, createRequest!.BaseclassName));
            }
            throw new NonWipProjectException(projectId);
        }

        public void DeleteClass(string projectId, string className)
        {
            Project project = GetProjectEntity(projectId);
            if (project is WipProject wipProject)
            {
                wipProject.RemoveClass(className);
                return;
            }
            throw new NonWipProjectException(projectId);
        }
    }
}
