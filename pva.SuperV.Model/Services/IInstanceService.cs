using pva.SuperV.Model.Instances;

namespace pva.SuperV.Model.Services
{
    public interface IInstanceService
    {
        Task<InstanceModel> CreateInstanceAsync(string projectId, InstanceModel createRequest);
        ValueTask DeleteInstanceAsync(string projectId, string instanceName);
        Task<InstanceModel> GetInstanceAsync(string projectId, string instanceName);
        Task<List<InstanceModel>> GetInstancesAsync(string projectId);
        Task<PagedSearchResult<InstanceModel>> SearchInstancesAsync(string projectId, InstancePagedSearchRequest search);
    }
}
