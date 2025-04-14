using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine;
using pva.SuperV.Model;
using pva.SuperV.Model.Classes;

namespace pva.SuperV.Api.Services.Classes
{
    public class ClassService : BaseService, IClassService
    {
        private readonly Dictionary<string, Comparison<ClassModel>> sortOptions = new()
            {
                { "name", new Comparison<ClassModel>((a, b) => a.Name.CompareTo(b.Name)) }
            };

        public List<ClassModel> GetClasses(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            return [.. project.Classes.Values.Select(clazz => ClassMapper.ToDto(clazz))];
        }

        public PagedSearchResult<ClassModel> SearchClasses(string projectId, ClassPagedSearchRequest search)
        {
            List<ClassModel> allClasses = GetClasses(projectId);
            List<ClassModel> projects = FilterClasses(allClasses, search);
            projects = SortResult(projects, search.SortOption, sortOptions);
            return CreateResult(search, allClasses, projects);
        }

        private static List<ClassModel> FilterClasses(List<ClassModel> allClasses, ClassPagedSearchRequest search)
        {
            List<ClassModel> filteredClasses = allClasses;
            if (!String.IsNullOrEmpty(search.NameFilter))
            {
                filteredClasses = [.. filteredClasses.Where(clazz => clazz.Name.Contains(search.NameFilter))];
            }
            return filteredClasses;
        }

        public ClassModel GetClass(string projectId, string className)
            => ClassMapper.ToDto(GetClassEntity(projectId, className));

        public ClassModel CreateClass(string projectId, ClassModel createRequest)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                return ClassMapper.ToDto(wipProject.AddClass(createRequest!.Name, createRequest!.BaseClassName));
            }
            throw new NonWipProjectException(projectId);
        }

        public ClassModel UpdateClass(string projectId, string className, ClassModel updateRequest)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                if (updateRequest.Name == null || updateRequest.Name.Equals(className))
                {
                    return ClassMapper.ToDto(wipProject.UpdateClass(className, updateRequest!.BaseClassName));
                }
                throw new EntityPropertyNotChangeableException("class", "Name");
            }
            throw new NonWipProjectException(projectId);
        }

        public void DeleteClass(string projectId, string className)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                wipProject.RemoveClass(className);
                return;
            }
            throw new NonWipProjectException(projectId);
        }
    }
}
