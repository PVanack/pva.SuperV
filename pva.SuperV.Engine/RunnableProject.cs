using pva.Helpers.Extensions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Engine.Processing;
using System.Text.Json.Serialization;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// A runnable project. It allows to create new instances. However its definitions are fixed and can't be changed.
    /// </summary>
    /// <seealso cref="pva.SuperV.Engine.Project" />
    public class RunnableProject : Project
    {
        /// <summary>
        /// The project assembly loader.
        /// </summary>
        [JsonIgnore]
        private ProjectAssemblyLoader? _projectAssemblyLoader;

        /// <summary>
        /// Gets the instances.
        /// </summary>
        /// <value>
        /// The instances.
        /// </value>
        [JsonIgnore]
        public Dictionary<string, dynamic> Instances { get; init; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="RunnableProject"/> class.
        /// </summary>
        public RunnableProject()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RunnableProject"/> class from a <see cref="WipProject"/>.
        /// </summary>
        /// <param name="wipProject">The wip project.</param>
        public RunnableProject(WipProject wipProject)
        {
            this.Name = wipProject.Name;
            this.Version = wipProject.Version;
            this.Classes = new(wipProject.Classes);
            this.FieldFormatters = new(wipProject.FieldFormatters);
            this.HistoryStorageEngineConnectionString = wipProject.HistoryStorageEngineConnectionString;
            this.HistoryStorageEngine = wipProject.HistoryStorageEngine;
            this.HistoryRepositories = new(wipProject.HistoryRepositories);
            CreateHistoryRepositories(wipProject.HistoryStorageEngine);
            CreateHistoryClassTimeSeries();
            this._projectAssemblyLoader = new();
            this._projectAssemblyLoader.LoadFromAssemblyPath(GetAssemblyFileName());
            RecreateInstances(wipProject);
        }

        private void CreateHistoryClassTimeSeries()
        {
            Classes.Values.ForEach(clazz =>
            {
                clazz.FieldDefinitions.Values.ForEach(fieldDefinition =>
                {
                    fieldDefinition.ValuePostChangeProcessings
                        .Where(vp => vp is IHistorizationProcessing)
                        .ForEach(vp =>
                            {
                                IHistorizationProcessing hp = vp as IHistorizationProcessing;
                                hp.UpsertInHistoryStorage(this.Name!, clazz.Name!);
                            });
                });
            });
        }

        public List<HistoryRow> GetHistoryValues(string instanceName, DateTime from, DateTime to, List<string> fieldNames)
        {
            IInstance instance = GetInstance(instanceName);
            List<IFieldDefinition> fields = [];
            HistoryRepository? historyRepository = null;
            string? classTimeSerieId = null;
            fieldNames.ForEach(fieldName => {
                IFieldDefinition field = instance.Class.GetField(fieldName);
                IHistorizationProcessing? hp =  field.ValuePostChangeProcessings
                    .Where(vp => vp is IHistorizationProcessing)
                    .Select(hp => hp as IHistorizationProcessing)
                    .FirstOrDefault();
                if (hp != null)
                {
                    historyRepository = hp.HistoryRepository;
                    classTimeSerieId = hp.ClassTimeSerieId;
                }
                fields.Add(field);
            });
            if (historyRepository is null || classTimeSerieId is null)
            {
                throw new NoHistoryStorageEngineException();
            }
            return HistoryStorageEngine!.GetHistoryValues(historyRepository.HistoryStorageId, classTimeSerieId!, instanceName, from, to, fields);
        }

        private void CreateHistoryRepositories(IHistoryStorageEngine? historyStorageEngine)
        {
            if (HistoryRepositories.Keys.Count > 0)
            {
                if (historyStorageEngine is null)
                {
                    throw new NoHistoryStorageEngineException(this.Name);
                }
                HistoryRepositories.Values.ForEach(repository =>
                    repository.UpsertRepository(Name!, historyStorageEngine));
            }
        }

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <returns>The newly created instance.</returns>
        /// <exception cref="pva.SuperV.Engine.Exceptions.InstanceAlreadyExistException"></exception>
        public dynamic? CreateInstance(string className, string instanceName)
        {
            if (Instances.ContainsKey(instanceName))
            {
                throw new InstanceAlreadyExistException(instanceName);
            }
            Class clazz = GetClass(className);
            string classFullName = $"{Name}.V{Version}.{clazz.Name}";
            dynamic? dynamicInstance = Activator.CreateInstanceFrom(GetAssemblyFileName(), classFullName)
                ?.Unwrap();
            if (dynamicInstance is not null)
            {
                dynamicInstance.Name = instanceName;
                dynamicInstance.Class = clazz;
                IInstance? instance = dynamicInstance as IInstance;
                clazz.FieldDefinitions.ForEach((k, v) =>
                {
                    instance!.Fields.TryGetValue(k, out IField? field);
                    field!.FieldDefinition = v;
                });
                Instances.Add(instanceName, dynamicInstance);
            }
            return dynamicInstance;
        }

        /// <summary>
        /// Removes an instance by its name.
        /// </summary>
        /// <param name="instanceName">Name of the instance.</param>
        public void RemoveInstance(string instanceName)
        {
            Instances.Remove(instanceName);
        }

        /// <summary>
        /// Gets an instance by its name.
        /// </summary>
        /// <param name="instanceName">Name of the instance.</param>
        /// <returns>The <see cref="Instance"/></returns>
        /// <exception cref="pva.SuperV.Engine.Exceptions.UnknownInstanceException"></exception>
        public Instance GetInstance(string instanceName)
        {
            if (Instances.TryGetValue(instanceName, out var instance))
            {
                return instance;
            }
            throw new UnknownInstanceException(instanceName);
        }

        /// <summary>
        /// Recreates the instances from a <see cref="WipProject"/>.
        /// </summary>
        /// <param name="wipProject">The wip project.</param>
        private void RecreateInstances(WipProject wipProject)
        {
            wipProject.ToLoadInstances
                .ForEach((k, v) =>
                {
                    string instanceName = k;
                    Instance oldInstance = v;
                    Instance? newInstance = CreateInstance(oldInstance!.Class!.Name!, instanceName!);
                    Dictionary<string, IField> newFields = new(newInstance!.Fields!.Count);
                    newInstance.Fields
                        .ForEach((k, v) =>
                        {
                            string fieldName = k;
                            if (oldInstance.Fields.TryGetValue(fieldName, out IField? oldField))
                            {
                                newFields.Add(fieldName, oldField);
                            }
                            else
                            {
                                newFields.Add(fieldName, v);
                            }
                        });
                    newInstance.Fields = newFields;
                });
        }

        /// <summary>
        /// Unloads the project. Clears all instances.
        /// </summary>
        public override void Unload()
        {
            Instances.Values.ForEach(instance =>
                instance.Dispose());
            Instances.Clear();
            _projectAssemblyLoader?.Unload();
            _projectAssemblyLoader = null;
            base.Unload();
        }
    }
}