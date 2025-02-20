using pva.SuperV.Engine;
using pva.SuperV.Model.Classes;

namespace pva.SuperV.Api
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
    }
}
