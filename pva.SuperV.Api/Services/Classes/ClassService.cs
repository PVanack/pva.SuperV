using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine;
using pva.SuperV.Model;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Services.Classes
{
    public class ClassService : BaseService, IClassService
    {
        private readonly ILogger logger;
        private readonly Dictionary<string, Comparison<ClassModel>> sortOptions = new()
            {
                { "name", new Comparison<ClassModel>((a, b) => a.Name.CompareTo(b.Name)) }
            };


        public ClassService(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger(this.GetType());
        }

        public async Task<List<ClassModel>> GetClassesAsync(string projectId)
        {
            logger.LogDebug("Getting classes for project {ProjectId}", projectId);
            Project project = GetProjectEntity(projectId);
            return await Task.FromResult(project.Classes.Values.Select(clazz => ClassMapper.ToDto(clazz)).ToList());
        }

        public async Task<PagedSearchResult<ClassModel>> SearchClassesAsync(string projectId, ClassPagedSearchRequest search)
        {
            logger.LogDebug("Searching classes for project {ProjectId} with filter {NameFilter} page number {PageNumber} page size {PageSize}",
                projectId, search.NameFilter, search.PageNumber, search.PageSize);
            List<ClassModel> allClasses = await GetClassesAsync(projectId);
            List<ClassModel> projects = FilterClasses(allClasses, search);
            projects = SortResult(projects, search.SortOption, sortOptions);
            return CreateResult(search, allClasses, projects);
        }

        public Task<ClassModel> GetClassAsync(string projectId, string className)
        {
            logger.LogDebug("Getting class {ClassName} for project {ProjectId}",
                className, projectId);
            return Task.FromResult(ClassMapper.ToDto(GetClassEntity(projectId, className)));
        }

        public Task<ClassModel> CreateClassAsync(string projectId, ClassModel createRequest)
        {
            logger.LogDebug("Creating class {ClassName} with base class {BaseClassName} for project {ProjectId}",
                createRequest.Name, createRequest.BaseClassName, projectId);
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                return Task.FromResult(ClassMapper.ToDto(wipProject.AddClass(createRequest!.Name, createRequest!.BaseClassName)));
            }
            return Task.FromException<ClassModel>(new NonWipProjectException(projectId));
        }

        public Task<ClassModel> UpdateClassAsync(string projectId, string className, ClassModel updateRequest)
        {
            logger.LogDebug("Updating class {ClassName} with base class {BaseClassName} for project {ProjectId}",
                updateRequest.Name, updateRequest.BaseClassName, projectId);
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                if (updateRequest.Name?.Equals(className) != false)
                {
                    return Task.FromResult(ClassMapper.ToDto(wipProject.UpdateClass(className, updateRequest!.BaseClassName)));
                }
                return Task.FromException<ClassModel>(new EntityPropertyNotChangeableException("class", "Name"));
            }
            return Task.FromException<ClassModel>(new NonWipProjectException(projectId));
        }

        public ValueTask DeleteClassAsync(string projectId, string className)
        {
            logger.LogDebug("Deleting class {ClassName} for project {ProjectId}",
                className, projectId);
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                wipProject.RemoveClass(className);
                return ValueTask.CompletedTask;
            }
            return ValueTask.FromException(new NonWipProjectException(projectId));
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
    }
}

