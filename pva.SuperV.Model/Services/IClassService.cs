using pva.SuperV.Model.Classes;

namespace pva.SuperV.Model.Services
{
    public interface IClassService
    {
        Task<List<ClassModel>> GetClassesAsync(string projectId);
        Task<ClassModel> GetClassAsync(string projectId, string className);
        Task<ClassModel> CreateClassAsync(string projectId, ClassModel createRequest);
        ValueTask DeleteClassAsync(string projectId, string className);
        Task<ClassModel> UpdateClassAsync(string projectId, string className, ClassModel updateRequest);
        Task<PagedSearchResult<ClassModel>> SearchClassesAsync(string projectId, ClassPagedSearchRequest search);
    }
}
