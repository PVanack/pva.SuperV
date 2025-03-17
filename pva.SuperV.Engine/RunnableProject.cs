using pva.Helpers.Extensions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.HistoryRetrieval;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Engine.Processing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
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

        [JsonIgnore]
        public WeakReference? ProjectAssemblyLoaderWeakRef
        {
            get => _projectAssemblyLoader == null
                ? null
                : new WeakReference(_projectAssemblyLoader, trackResurrection: true);
        }

        /// <summary>
        /// Gets the instances.
        /// </summary>
        /// <value>
        /// The instances.
        /// </value>
        [JsonIgnore]
        public Dictionary<string, Instance> Instances { get; } = new(StringComparer.OrdinalIgnoreCase);

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
            Name = wipProject.Name;
            Version = wipProject.Version;
            Classes = new(wipProject.Classes);
            FieldFormatters = new(wipProject.FieldFormatters);
            HistoryStorageEngineConnectionString = wipProject.HistoryStorageEngineConnectionString;
            HistoryStorageEngine = wipProject.HistoryStorageEngine;
            HistoryRepositories = new(wipProject.HistoryRepositories);
            CreateHistoryRepositories(wipProject.HistoryStorageEngine);
            CreateHistoryClassTimeSeries();
            SetupProjectAssemblyLoader();
            RecreateInstances(wipProject);
        }

        public override string GetId()
        {
            return $"{Name!}";
        }

        private void SetupProjectAssemblyLoader()
        {
            Task.Run(async () => await ProjectBuilder.BuildAsync(this)).Wait();
            _projectAssemblyLoader ??= new();
            _projectAssemblyLoader.LoadFromAssemblyPath(GetAssemblyFileName());
        }

        private void CreateHistoryClassTimeSeries()
        {
            Classes.Values.ForEach(clazz =>
            {
                clazz.FieldDefinitions.Values.ForEach(fieldDefinition =>
                {
                    fieldDefinition.ValuePostChangeProcessings
                        .OfType<IHistorizationProcessing>()
                        .ForEach(hp =>
                            hp.UpsertInHistoryStorage(Name!, clazz.Name!));
                });
            });
        }

        private void CreateHistoryRepositories(IHistoryStorageEngine? historyStorageEngine)
        {
            if (HistoryRepositories.Keys.Count == 0)
            {
                return;
            }
            if (historyStorageEngine is null)
            {
                throw new NoHistoryStorageEngineException(Name);
            }

            HistoryRepositories.Values.ForEach(repository =>
                repository.UpsertRepository(Name!, historyStorageEngine));
        }

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <returns>The newly created instance.</returns>
        /// <exception cref="pva.SuperV.Engine.Exceptions.EntityAlreadyExistException"></exception>
        public Instance? CreateInstance(string className, string instanceName)
        {
            SetupProjectAssemblyLoader();
            if (Instances.ContainsKey(instanceName))
            {
                throw new EntityAlreadyExistException("Instance", instanceName);
            }

            Class clazz = GetClass(className);
            string classFullName = $"{Name}.V{Version}.{clazz.Name}";
            Type? classType = _projectAssemblyLoader?.Assemblies.First()?.GetType(classFullName!);

            Instance? instance = CreateInstance(classType!);
            if (instance is null)
            {
                return instance;
            }
            instance.Name = instanceName;
            instance.Class = clazz;
            Class? currentClass = clazz;
            while (currentClass is not null)
            {
                currentClass.FieldDefinitions.ForEach((k, v) =>
                {
                    instance!.Fields.TryGetValue(k, out IField? field);
                    field!.FieldDefinition = v;
                });
                currentClass = currentClass.BaseClass;
            }
            Instances.Add(instanceName, instance);
            return instance;
        }

        /// <summary>
        /// Creates an instance for targetType's <see cref="FieldDefinition{T}"/>.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns><see cref="IFieldDefinition"/> created instance.</returns>
        private static Instance CreateInstance(Type targetType)
        {
            var ctor = GetConstructor(targetType);
            return (Instance)ctor.Invoke([]);
        }

        /// <summary>
        /// Gets the constructor for targetType's <see cref="FieldDefinition{T}"/>.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="argumentType">Type of the argument.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">No constructor found for FieldDefinition{targetType.Name}.</exception>
        private static ConstructorInfo GetConstructor(Type targetType)
        {
            return targetType.GetConstructor(Type.EmptyTypes)
                ?? throw new InvalidOperationException($"No constructor found for {targetType.Name}.");
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

            throw new UnknownEntityException("Instance", instanceName);
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
                    Instance? newInstance = CreateInstance(oldInstance.Class.Name!, instanceName);
                    Dictionary<string, IField> newFields = new(newInstance!.Fields.Count);
                    newInstance.Fields
                        .ForEach((k1, v2) =>
                        {
                            string fieldName = k1;
                            newFields.Add(fieldName,
                                oldInstance.Fields.GetValueOrDefault(fieldName, v2));
                        });
                    newInstance.Fields = newFields;
                });
        }

        /// <summary>
        /// Unloads the project. Clears all instances.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public override void Unload()
        {
            Instances.Values.ForEach(instance =>
                instance.Dispose());
            Instances.Clear();
            base.Unload();
            Projects.Remove(GetId(), out _);
            _projectAssemblyLoader?.Unload();
            _projectAssemblyLoader = null;
        }

        /// <summary>
        /// Gets the C# code for generating the project's assembly with <see cref="Project.BuildAsync(WipProject)"/>.
        /// </summary>
        /// <returns>C# code.</returns>
        public string GetCode()
        {
            StringBuilder codeBuilder = new();
            codeBuilder.AppendLine($"using {GetType().Namespace};");
            codeBuilder.AppendLine("using System.Collections.Generic;");
            codeBuilder.AppendLine("using System.Reflection;");
            codeBuilder.AppendLine($"[assembly: AssemblyProduct(\"{Name}\")]");
            codeBuilder.AppendLine($"[assembly: AssemblyTitle(\"{Description}\")]");
            codeBuilder.AppendLine($"[assembly: AssemblyVersion(\"{Version}\")]");
            codeBuilder.AppendLine($"[assembly: AssemblyFileVersion(\"{Version}\")]");
            codeBuilder.AppendLine($"[assembly: AssemblyInformationalVersion(\"{Version}\")]");
            codeBuilder.AppendLine($"namespace {Name}.V{Version} {{");
            Classes
                .ForEach((_, v) => codeBuilder.AppendLine(v.GetCode()));
            codeBuilder.AppendLine("}");
            return codeBuilder.ToString();
        }

        public void SetInstanceValue<T>(string instanceName, string fieldName, T fieldValue)
        {
            SetInstanceValue(instanceName, fieldName, fieldValue, DateTime.UtcNow, QualityLevel.Good);
        }

        public void SetInstanceValue<T>(string instanceName, string fieldName, T fieldValue, DateTime timestamp)
        {
            SetInstanceValue(instanceName, fieldName, fieldValue, timestamp, QualityLevel.Good);
        }

        public void SetInstanceValue<T>(string instanceName, string fieldName, T fieldValue, QualityLevel qualityLevel)
        {
            SetInstanceValue(instanceName, fieldName, fieldValue, DateTime.UtcNow, qualityLevel);
        }

        public void SetInstanceValue<T>(string instanceName, string fieldName, T fieldValue, DateTime timestamp,
            QualityLevel qualityLevel)
        {
            Instance instance = GetInstance(instanceName);
            IField field = instance.GetField(fieldName);
            if (field is not Field<T> typedField)
            {
                throw new WrongFieldTypeException(fieldName);
            }

            typedField.SetValue(fieldValue, timestamp, qualityLevel);
        }

        public List<HistoryRow> GetHistoryValues(string instanceName, HistoryTimeRange query, List<string> fieldNames)
        {
            Instance instance = GetInstance(instanceName);
            GetHistoryParametersForFields(instance, fieldNames,
                out List<IFieldDefinition> fields, out HistoryRepository? historyRepository, out string? classTimeSerieId);

            return GetHistoryValues(instanceName, query, fields, historyRepository!, classTimeSerieId!);
        }

        public List<HistoryRow> GetHistoryValues(string instanceName, HistoryTimeRange query, List<IFieldDefinition> fields,
            HistoryRepository historyRepository, string classTimeSerieId)
        {
            return HistoryStorageEngine!.GetHistoryValues(historyRepository!.HistoryStorageId!, classTimeSerieId!,
                instanceName, query, fields);
        }

        public List<HistoryStatisticRow> GetHistoryStatistics(string instanceName, HistoryStatisticTimeRange query, List<HistoryStatisticFieldName> fieldNames)
        {
            Instance instance = GetInstance(instanceName);
            GetHistoryParametersForFields(instance, [.. fieldNames.Select(field => field.Name)],
                out List<IFieldDefinition> fields, out HistoryRepository? historyRepository, out string? classTimeSerieId);
            List<HistoryStatisticField> statFields = [];
            for (int index = 0; index < fieldNames.Count; index++)
            {
                statFields.Add(new(fields[index], fieldNames[index].StatisticFunction));
            }
            return GetHistoryStatistics(instanceName, query, statFields, historyRepository!, classTimeSerieId!);
        }

        public List<HistoryStatisticRow> GetHistoryStatistics(string instanceName, HistoryStatisticTimeRange query, List<HistoryStatisticField> fields,
            HistoryRepository historyRepository, string classTimeSerieId)
        {
            return HistoryStorageEngine!.GetHistoryStatistics(historyRepository!.HistoryStorageId!, classTimeSerieId!,
                instanceName, query, fields);
        }


        public static void GetHistoryParametersForFields(Instance instance, List<string> fieldNames,
            out List<IFieldDefinition> fields, out HistoryRepository? historyRepository, out string? classTimeSerieId)
        {
            fields = [];
            historyRepository = null;
            classTimeSerieId = null;
            foreach (string fieldName in fieldNames)
            {
                IFieldDefinition field = instance.Class.GetField(fieldName);
                IHistorizationProcessing? hp = field.ValuePostChangeProcessings
                    .OfType<IHistorizationProcessing>()
                    .FirstOrDefault();
                if (hp != null)
                {
                    historyRepository = hp.HistoryRepository;
                    classTimeSerieId = hp.ClassTimeSerieId;
                }
                fields.Add(field);
            }
            if (historyRepository is null || classTimeSerieId is null)
            {
                throw new NoHistoryStorageEngineException();
            }
        }
    }
}