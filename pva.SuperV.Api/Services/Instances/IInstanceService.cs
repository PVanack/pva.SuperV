using pva.SuperV.Model.Instances;

namespace pva.SuperV.Api.Services.Instances
{
    public interface IInstanceService
    {
        InstanceModel CreateInstance(string projectId, string className, string instanceName);
        void DeleteInstance(string projectId, string instanceName);
        InstanceModel GetInstance(string projectId, string instanceName);
        List<InstanceModel> GetInstances(string projectId);
    }
}
