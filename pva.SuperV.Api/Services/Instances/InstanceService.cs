using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.Instances;

namespace pva.SuperV.Api.Services.Instances
{
    public class InstanceService : BaseService, IInstanceService
    {
        private readonly Dictionary<string, Comparison<InstanceModel>> sortOptions = new()
            {
                { "name", new Comparison<InstanceModel>((a, b) => a.Name.CompareTo(b.Name)) }
            };

        public List<InstanceModel> GetInstances(string projectId)
        {
            if (GetProjectEntity(projectId) is RunnableProject runnableProject)
            {
                return [.. runnableProject.Instances.Values.Select(InstanceMapper.ToDto)];
            }
            throw new NonRunnableProjectException(projectId);
        }

        public PagedSearchResult<InstanceModel> SearchInstances(string projectId, InstancePagedSearchRequest search)
        {
            List<InstanceModel> allInstances = GetInstances(projectId);
            List<InstanceModel> projects = FilterInstances(projectId, allInstances, search);
            projects = SortResult(projects, search.SortOption, sortOptions);
            return CreateResult(search, allInstances, projects);

        }

        private static List<InstanceModel> FilterInstances(string projectId, List<InstanceModel> allInstances, InstancePagedSearchRequest search)
        {
            List<InstanceModel> filteredInstances = allInstances;
            if (!String.IsNullOrEmpty(search.ClassName))
            {
                Dictionary<string, bool> classNameMatches = [];
                filteredInstances = [.. filteredInstances.Where(instance => FilterInstanceClass(projectId, search.ClassName, instance, ref classNameMatches))];
            }
            if (!String.IsNullOrEmpty(search.NameFilter))
            {
                filteredInstances = [.. filteredInstances.Where(instance => instance.Name.Contains(search.NameFilter))];
            }
            return filteredInstances;
        }

        private static bool FilterInstanceClass(string projectId, string searchedClassName, InstanceModel instance, ref Dictionary<string, bool> classNameMatches)
        {
            bool isClassNameMatching;
            if (classNameMatches.TryGetValue(instance.ClassName, out isClassNameMatching))
            {
                return isClassNameMatching;
            }
            Class? clazz = GetClassEntity(projectId, instance.ClassName);
            List<string> classInheritance = [];
            while (clazz != null)
            {
                isClassNameMatching = clazz.Name.Equals(searchedClassName);
                classInheritance.Add(clazz.Name);
                if (isClassNameMatching)
                {
                    break;
                }
                clazz = clazz.BaseClass;
            }
            foreach (string className in classInheritance)
            {
                classNameMatches[className] = isClassNameMatching;
            }
            return isClassNameMatching;
        }

        public InstanceModel GetInstance(string projectId, string instanceName)
        {
            if (GetProjectEntity(projectId) is RunnableProject runnableProject)
            {
                if (runnableProject.Instances.TryGetValue(instanceName, out Instance? instance))
                {
                    return InstanceMapper.ToDto(instance);
                }
                throw new UnknownEntityException("Instance", instanceName);
            }
            throw new NonRunnableProjectException(projectId);
        }

        public InstanceModel CreateInstance(string projectId, InstanceModel createRequest)
        {
            if (GetProjectEntity(projectId) is RunnableProject runnableProject)
            {
                Instance? createdInstance = runnableProject.CreateInstance(createRequest.ClassName, createRequest.Name);
                createRequest.Fields.ForEach(fieldModel =>
                {
                    IField? field = createdInstance!.GetField(fieldModel.Name) ?? throw new UnknownEntityException("Field", fieldModel.Name);
                    FieldValueMapper.SetFieldValue(field, fieldModel.FieldValue);
                });
                return InstanceMapper.ToDto(createdInstance!);
            }
            throw new NonRunnableProjectException(projectId);
        }

        public void DeleteInstance(string projectId, string instanceName)
        {
            if (GetProjectEntity(projectId) is RunnableProject runnableProject)
            {
                runnableProject.RemoveInstance(instanceName);
                return;
            }
            throw new NonRunnableProjectException(projectId);
        }
    }
}
