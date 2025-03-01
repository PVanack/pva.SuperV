using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Instances;

namespace pva.SuperV.Api.Services.Instances
{
    public class InstanceService : BaseService, IInstanceService
    {
        public List<InstanceModel> GetInstances(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            if (project is RunnableProject runnableProject)
            {
                return [.. runnableProject.Instances.Values.Select(InstanceMapper.ToDto)];
            }
            throw new NonRunnableProjectException(projectId);
        }

        public InstanceModel GetInstance(string projectId, string instanceName)
        {
            Project project = GetProjectEntity(projectId);
            if (project is RunnableProject runnableProject)
            {
                if (runnableProject.Instances.TryGetValue(instanceName, out Instance? instance))
                {
                    return InstanceMapper.ToDto(instance);
                }
                throw new UnknownEntityException("Instance", instanceName);
            }
            throw new NonRunnableProjectException(projectId);
        }

        public InstanceModel CreateInstance(string projectId, string className, string instanceName)
        {
            Project project = GetProjectEntity(projectId);
            if (project is RunnableProject runnableProject)
            {
                Instance? createdInstance = runnableProject.CreateInstance(className, instanceName);
                return InstanceMapper.ToDto(createdInstance!);
            }
            throw new NonRunnableProjectException(projectId);
        }

        public void DeleteInstance(string projectId, string instanceName)
        {
            Project project = GetProjectEntity(projectId);
            if (project is RunnableProject runnableProject)
            {
                runnableProject.RemoveInstance(instanceName);
                return;
            }
            throw new NonRunnableProjectException(projectId);
        }

    }
}
