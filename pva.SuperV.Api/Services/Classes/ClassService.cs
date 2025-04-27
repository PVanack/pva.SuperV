using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine;
using pva.SuperV.Model;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Services.Classes
{
    public class ClassService : BaseService, IClassService
    {
        private readonly Dictionary<string, Comparison<ClassModel>> sortOptions = new()
            {
                { "name", new Comparison<ClassModel>((a, b) => a.Name.CompareTo(b.Name)) }
            };

        public async Task<List<ClassModel>> GetClassesAsync(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            return await Task.FromResult(project.Classes.Values.Select(clazz => ClassMapper.ToDto(clazz)).ToList());
        }

        public async Task<PagedSearchResult<ClassModel>> SearchClassesAsync(string projectId, ClassPagedSearchRequest search)
        {
            List<ClassModel> allClasses = await GetClassesAsync(projectId);
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

        public Task<ClassModel> GetClassAsync(string projectId, string className)
            => Task.FromResult(ClassMapper.ToDto(GetClassEntity(projectId, className)));

        public Task<ClassModel> CreateClassAsync(string projectId, ClassModel createRequest)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                return Task.FromResult(ClassMapper.ToDto(wipProject.AddClass(createRequest!.Name, createRequest!.BaseClassName)));
            }
            return Task.FromException<ClassModel>(new NonWipProjectException(projectId));
        }

        public Task<ClassModel> UpdateClassAsync(string projectId, string className, ClassModel updateRequest)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                if (updateRequest.Name == null || updateRequest.Name.Equals(className))
                {
                    return Task.FromResult(ClassMapper.ToDto(wipProject.UpdateClass(className, updateRequest!.BaseClassName)));
                }
                return Task.FromException<ClassModel>(new EntityPropertyNotChangeableException("class", "Name"));
            }
            return Task.FromException<ClassModel>(new NonWipProjectException(projectId));
        }

        public ValueTask DeleteClassAsync(string projectId, string className)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                wipProject.RemoveClass(className);
                return ValueTask.CompletedTask;
            }
            return ValueTask.FromException(new NonWipProjectException(projectId));
        }
    }
}
