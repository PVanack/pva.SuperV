using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model;
using pva.SuperV.Model.Instances;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Services.Instances
{
    public class InstanceService : BaseService, IInstanceService
    {
        private readonly ILogger logger;
        private readonly Dictionary<string, Comparison<InstanceModel>> sortOptions = new()
            {
                { "name", new Comparison<InstanceModel>((a, b) => a.Name.CompareTo(b.Name)) }
            };

        public InstanceService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(this.GetType());
        }

        public async Task<List<InstanceModel>> GetInstancesAsync(string projectId)
        {
            logger.LogDebug("Getting instances for project {ProjectId}",
                projectId);
            if (GetProjectEntity(projectId) is RunnableProject runnableProject)
            {
                return await Task.FromResult(runnableProject.Instances.Values.Select(InstanceMapper.ToDto).ToList());
            }
            return await Task.FromException<List<InstanceModel>>(new NonRunnableProjectException(projectId));
        }

        public async Task<PagedSearchResult<InstanceModel>> SearchInstancesAsync(string projectId, InstancePagedSearchRequest search)
        {
            logger.LogDebug("Searching instances for project {ProjectId} with filter {NameFilter} class {ClassNameFilter} page number {PageNumber} page size {PageSize}",
                projectId, search.NameFilter, search.ClassName, search.PageNumber, search.PageSize);
            List<InstanceModel> allInstances = await GetInstancesAsync(projectId);
            List<InstanceModel> projects = FilterInstances(projectId, allInstances, search);
            projects = SortResult(projects, search.SortOption, sortOptions);
            return CreateResult(search, allInstances, projects);

        }

        public async Task<InstanceModel> GetInstanceAsync(string projectId, string instanceName)
        {
            logger.LogDebug("Getting instance {InstanceName} for project {ProjectId}",
                instanceName, projectId);
            if (GetProjectEntity(projectId) is RunnableProject runnableProject)
            {
                if (runnableProject.Instances.TryGetValue(instanceName, out Instance? instance))
                {
                    return await Task.FromResult(InstanceMapper.ToDto(instance));
                }
                return await Task.FromException<InstanceModel>(new UnknownEntityException("Instance", instanceName));
            }
            return await Task.FromException<InstanceModel>(new NonRunnableProjectException(projectId));
        }

        public async Task<InstanceModel> CreateInstanceAsync(string projectId, InstanceModel createRequest, bool addToRunningInstances = true)
        {
            logger.LogDebug("Creating instance {InstanceName} of class {ClassName} with fields {Fields} for project {ProjectId}",
                createRequest.Name, createRequest.ClassName, String.Join(",", createRequest.Fields.Select(field => string.Format($"{field.Name}={field.FieldValue.FormattedValue}").ToList())), projectId);
            if (GetProjectEntity(projectId) is RunnableProject runnableProject)
            {
                Instance? createdInstance = runnableProject.CreateInstance(createRequest.ClassName, createRequest.Name, addToRunningInstances);
                createRequest.Fields.ForEach(fieldModel =>
                {
                    IField? field = createdInstance!.GetField(fieldModel.Name) ?? throw new UnknownEntityException("Field", fieldModel.Name);
                    FieldValueMapper.SetFieldValue(field, fieldModel.FieldValue);
                });
                return await Task.FromResult(InstanceMapper.ToDto(createdInstance!));
            }
            return await Task.FromException<InstanceModel>(new NonRunnableProjectException(projectId));
        }

        public async ValueTask DeleteInstanceAsync(string projectId, string instanceName)
        {
            logger.LogDebug("Deleting instance {InstanceName} for project {ProjectId}",
                instanceName, projectId);
            if (GetProjectEntity(projectId) is RunnableProject runnableProject)
            {
                runnableProject.RemoveInstance(instanceName);
                await ValueTask.CompletedTask;
                return;
            }
            await ValueTask.FromException(new NonRunnableProjectException(projectId));
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
            if (classNameMatches.TryGetValue(instance.ClassName, out bool isClassNameMatching))
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
    }
}
